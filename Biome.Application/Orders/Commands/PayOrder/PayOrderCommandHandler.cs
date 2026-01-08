using Biome.Domain.Orders;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Orders.Commands.PayOrder;

internal sealed class PayOrderCommandHandler : IRequestHandler<PayOrderCommand, Result>
{
    private readonly IOrderRepository _orderRepository;

    public PayOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result> Handle(PayOrderCommand request, CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure(new Error("Order.NotFound", "Order not found."));
        }

        Result result = order.MarkAsPaid();

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
