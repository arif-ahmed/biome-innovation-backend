namespace Biome.Domain.Users.Events;

using Biome.SharedKernel.Abstractions;

public sealed record UserEmailVerifiedDomainEvent(Guid UserId) : IDomainEvent;
