using ProductCatalog.Application.Abstractions.Email;

namespace ProductCatalog.Infrastructure.Email;

internal sealed class EmailService : IEmailService
{
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
