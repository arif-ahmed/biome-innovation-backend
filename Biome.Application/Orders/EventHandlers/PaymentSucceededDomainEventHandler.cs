using Biome.Domain.Orders;
using Biome.Domain.Payments.Events;
using MediatR;

namespace Biome.Application.Orders.EventHandlers;

public sealed class PaymentSucceededDomainEventHandler : INotificationHandler<PaymentSucceededDomainEvent>
{
    private readonly IOrderRepository _orderRepository;

    public PaymentSucceededDomainEventHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Handle(PaymentSucceededDomainEvent notification, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(notification.OrderId, cancellationToken);
        if (order is null)
        {
            // Log warning: Payment succeeded for non-existent order?
            return;
        }

        var result = order.MarkAsPaid();
        
        if (result.IsSuccess)
        {
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
