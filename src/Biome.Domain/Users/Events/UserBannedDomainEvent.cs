namespace Biome.Domain.Users.Events;

using Biome.SharedKernel.Abstractions;

public sealed record UserBannedDomainEvent(Guid UserId) : IDomainEvent;
