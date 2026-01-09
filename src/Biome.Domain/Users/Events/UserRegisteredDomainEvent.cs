namespace Biome.Domain.Users.Events;

using Biome.SharedKernel.Abstractions;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
