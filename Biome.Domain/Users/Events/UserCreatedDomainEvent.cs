namespace Biome.Domain.Users.Events;

using Biome.SharedKernel.Abstractions;

public sealed record UserCreatedDomainEvent(Guid UserId, string TemporaryPassword) : IDomainEvent;
