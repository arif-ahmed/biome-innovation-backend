using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Primitives;

namespace Biome.Domain.Users.Events;

public sealed record UserPasswordResetRequestedDomainEvent(Guid UserId, string ResetToken) : IDomainEvent;
