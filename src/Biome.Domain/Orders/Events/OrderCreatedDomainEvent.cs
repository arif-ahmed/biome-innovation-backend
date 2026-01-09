using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Primitives;

namespace Biome.Domain.Orders.Events;

public sealed record OrderCreatedDomainEvent(Guid OrderId, Guid CustomerId) : IDomainEvent;
