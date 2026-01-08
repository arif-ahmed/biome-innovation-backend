using Biome.Domain.Orders;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Orders.Commands.CreateOrder;

internal sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Initialize Order
        var order = Order.Create(request.CustomerId);

        // 2. Add Items
        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity, item.KitType, item.PetId);
        }

        // 3. Finalize Creation (Invariant Check: Must have items)
        Result finalizationResult = order.FinalizeCreation();
        if (finalizationResult.IsFailure)
        {
            return Result.Failure<Guid>(finalizationResult.Error);
        }

        // 4. Persist
        _orderRepository.Add(order);
        await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
