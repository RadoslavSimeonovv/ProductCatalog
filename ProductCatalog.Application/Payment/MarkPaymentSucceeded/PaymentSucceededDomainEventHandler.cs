using MediatR;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Repositories;
using ProductCatalog.Domain.Payment.Events;

namespace ProductCatalog.Application.Payment.MarkPaymentSucceeded;

internal sealed class PaymentSucceededDomainEventHandler : INotificationHandler<PaymentSucceededDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentSucceededDomainEventHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(PaymentSucceededDomainEvent notification, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(notification.OrderId, cancellationToken);

        if (order == null)
            return;

        var result = order.MarkAsPaid();
        if (result.IsFailure)
            return;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
