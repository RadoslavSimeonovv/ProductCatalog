using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Abstractions.Email;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Enums;
using ProductCatalog.Domain.Order.Repositories;
using ProductCatalog.Domain.Payment.Events;

namespace ProductCatalog.Application.Payment.MarkPaymentSucceeded;

internal sealed class PaymentSucceededDomainEventHandler : INotificationHandler<PaymentSucceededDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<PaymentSucceededDomainEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentSucceededDomainEventHandler(
        IOrderRepository orderRepository,
        IEmailService emailService,
        ILogger<PaymentSucceededDomainEventHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _emailService = emailService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(PaymentSucceededDomainEvent notification, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(notification.OrderId, cancellationToken);

        if (order is null)
            return;

        if (order.Status == OrderStatus.Paid)
            return;

        var result = order.MarkAsPaid();
        if (result.IsFailure)
            return;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await _emailService.SendAsync(
                new EmailMessage(
                    To: order.CustomerEmail,
                    Subject: "Order paid",
                    HtmlBody: "<p>Your order was paid successfully.</p>"),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send payment succeeded email for order {OrderId}", order.Id);
        }
    }
}