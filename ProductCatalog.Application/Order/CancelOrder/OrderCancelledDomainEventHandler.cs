using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Abstractions.Email;
using ProductCatalog.Domain.Order.Events;

namespace ProductCatalog.Application.Order.CancelOrder;

internal sealed class OrderCancelledDomainEventHandler : INotificationHandler<OrderCancelledDomainEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<OrderCancelledDomainEventHandler> _logger;
    public OrderCancelledDomainEventHandler(
        IEmailService emailService,
        ILogger<OrderCancelledDomainEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(OrderCancelledDomainEvent notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notification.CustomerEmail))
            return;

        var reason = string.IsNullOrWhiteSpace(notification.Reason) ? "No reason provided." : notification.Reason;

        var html =
            $"<p>Dear customer,</p>" +
            $"<p>Your order <strong>{notification.OrderId}</strong> has been cancelled.</p>" +
            $"<p>Reason: {reason}</p>" +
            $"<p>Best regards,<br/>Product Catalog Team</p>";

        try
        {
            await _emailService.SendAsync(
                new EmailMessage(notification.CustomerEmail, "Your order has been cancelled", html),
                cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send order cancellation email for OrderId: {OrderId} to {Email}", notification.OrderId, notification.CustomerEmail);
        }
    }
}