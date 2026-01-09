using Biome.Domain.Notifications;
using Biome.Domain.Notifications.Events;
using Biome.Domain.Users;
using Biome.SharedKernel.Abstractions;
using MediatR;

namespace Biome.Application.Notifications.EventHandlers;

public sealed class NotificationCreatedEventHandler : INotificationHandler<NotificationCreatedDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public NotificationCreatedEventHandler(
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task Handle(NotificationCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var notif = await _notificationRepository.GetByIdAsync(notification.NotificationId, cancellationToken);
        if (notif is null) return;

        var user = await _userRepository.GetByIdAsync(notif.UserId, cancellationToken);
        if (user is null) return;

        try
        {
            await _emailService.SendEmailAsync(
                user.Email.Value, 
                "Biome Innovation Notification", 
                notif.Message, 
                cancellationToken);

            notif.MarkAsSent();
        }
        catch
        {
            notif.MarkAsFailed();
        }

        await _notificationRepository.UpdateAsync(notif, cancellationToken);
        await _notificationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
