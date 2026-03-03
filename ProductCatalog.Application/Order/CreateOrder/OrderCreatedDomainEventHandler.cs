using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Abstractions.Email;
using ProductCatalog.Domain.Order.Events;

namespace ProductCatalog.Application.Order.CreateOrder;

internal sealed class OrderCreatedDomainEventHandler : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<OrderCreatedDomainEventHandler> _logger;

    public OrderCreatedDomainEventHandler(
        IEmailService emailService,
        ILogger<OrderCreatedDomainEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }
    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notification.CustomerEmail))
            return;

        var html =
            $"<p>Dear customer,</p>" +
            $"<p>Your order with ID <strong>{notification.OrderId}</strong> has been created.</p>" +
            $"<p>Total: <strong>{notification.Total.Amount} {notification.Total.Currency.Code}</strong></p>" +
            $"<p>Thank you for shopping with us!</p>";

        try
        {
            await _emailService.SendAsync(
                new EmailMessage(notification.CustomerEmail, "Your order has been created", html),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send order created email for order {OrderId} to {CustomerEmail}", notification.OrderId, notification.CustomerEmail);
        }
    }
}
