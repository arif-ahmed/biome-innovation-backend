using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Primitives;

namespace Biome.Domain.Orders.Events;

public sealed record OrderPaidDomainEvent(Guid OrderId) : IDomainEvent;
