using Biome.Application.Orders.Common;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Orders.Queries.GetMyOrders;

public sealed record GetMyOrdersQuery(Guid UserId) : IRequest<Result<List<OrderResponse>>>;
