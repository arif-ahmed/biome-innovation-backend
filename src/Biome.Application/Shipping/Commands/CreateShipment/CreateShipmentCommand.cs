using Biome.SharedKernel.Enums;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Shipping.Commands.CreateShipment;

public sealed record CreateShipmentCommand(Guid OrderId, Carrier Carrier, string DestinationAddress) : IRequest<Result<Guid>>;
