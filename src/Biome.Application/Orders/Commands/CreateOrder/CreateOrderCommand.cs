using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(Guid CustomerId, List<OrderItemDto> Items) : IRequest<Result<Guid>>;
