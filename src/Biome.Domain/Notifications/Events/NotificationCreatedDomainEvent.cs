using Biome.SharedKernel.Abstractions;

namespace Biome.Domain.Notifications.Events;

public sealed record NotificationCreatedDomainEvent(Guid NotificationId) : IDomainEvent;
