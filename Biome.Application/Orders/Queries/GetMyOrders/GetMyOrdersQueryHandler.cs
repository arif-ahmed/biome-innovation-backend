using Biome.Application.Orders.Common;
using Biome.Domain.Orders;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Orders.Queries.GetMyOrders;

internal sealed class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, Result<List<OrderResponse>>>
{
    private readonly IOrderRepository _orderRepository;

    public GetMyOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<List<OrderResponse>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByCustomerIdAsync(request.UserId, cancellationToken);

        var response = orders.Select(o => new OrderResponse(
            o.Id,
            o.OrderDate,
            o.Status,
            o.TotalAmount,
            o.OrderItems.Select(i => new OrderItemResponse(
                i.ProductId,
                i.ProductName,
                i.UnitPrice,
                i.Quantity,
                i.KitType,
                i.PetId,
                i.TotalAmount)).ToList()
        )).ToList();

        return response;
    }
}
