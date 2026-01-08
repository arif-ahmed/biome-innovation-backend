using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Orders.Commands.PayOrder;

public sealed record PayOrderCommand(Guid OrderId) : IRequest<Result>;
