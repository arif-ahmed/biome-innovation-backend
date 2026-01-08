using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Primitives;

namespace Biome.Domain.Users.Events;

public sealed record UserPasswordChangedDomainEvent(Guid UserId) : IDomainEvent;
