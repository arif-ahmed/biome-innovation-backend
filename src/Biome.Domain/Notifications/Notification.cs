using Biome.Domain.Notifications.Enums;
using Biome.Domain.Notifications.Events;
using Biome.SharedKernel.Core;

namespace Biome.Domain.Notifications;

public sealed class Notification : AggregateRoot
{
    private Notification(Guid id, Guid userId, NotificationType type, string message) : base(id)
    {
        UserId = userId;
        Type = type;
        Message = message;
        Status = NotificationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Message { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }

    public static Notification Create(Guid userId, NotificationType type, string message)
    {
        var notification = new Notification(Guid.NewGuid(), userId, type, message);
        notification.RaiseDomainEvent(new NotificationCreatedDomainEvent(notification.Id));
        return notification;
    }

    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = NotificationStatus.Failed;
    }
}
