using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProductCatalog.Domain.Abstractions;
using Quartz;

namespace ProductCatalog.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxMessageJob : IJob
{
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly IPublisher _publisher;
    private readonly ApplicationDbContext _dbContext;
    private readonly OutboxOptions _outboxOptions;
    private readonly ILogger<ProcessOutboxMessageJob> _logger;

    public ProcessOutboxMessageJob(
        IPublisher publisher,
        ApplicationDbContext dbContext,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<ProcessOutboxMessageJob> logger)
    {
        _publisher = publisher;
        _dbContext = dbContext;
        _outboxOptions = outboxOptions.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await _dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(_outboxOptions.BatchSize)
            .ToListAsync(context.CancellationToken);

        if (messages.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Processing {Count} outbox messages.", messages.Count);

        foreach (var message in messages)
        {
            try
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(message.Content, JsonSettings);

                if (domainEvent is null)
                {
                    _logger.LogWarning("Failed to deserialize outbox message {MessageId} of type {Type}.", message.Id, message.Type);
                    continue;
                }

                await _publisher.Publish(domainEvent, context.CancellationToken);

                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox message {MessageId} of type {Type}.", message.Id, message.Type);

                message.Error = ex.Message;
                message.ProcessedOnUtc = DateTime.UtcNow;
            }
        }

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Processed {Count} outbox messages.", messages.Count);
    }
}
