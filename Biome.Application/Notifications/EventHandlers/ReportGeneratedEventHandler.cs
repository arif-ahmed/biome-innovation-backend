using Biome.Domain.Notifications;
using Biome.Domain.Notifications.Enums;
using Biome.Domain.Pets;
using Biome.Domain.Reports.Events;
using MediatR;

namespace Biome.Application.Notifications.EventHandlers;

public sealed class ReportGeneratedEventHandler : INotificationHandler<ReportGeneratedDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IPetRepository _petRepository;

    public ReportGeneratedEventHandler(
        INotificationRepository notificationRepository,
        IPetRepository petRepository)
    {
        _notificationRepository = notificationRepository;
        _petRepository = petRepository;
    }

    public async Task Handle(ReportGeneratedDomainEvent notification, CancellationToken cancellationToken)
    {
        var pet = await _petRepository.GetByIdAsync(notification.PetId, cancellationToken);
        if (pet is null) return;

        var notif = Notification.Create(
            pet.OwnerId,
            NotificationType.ReportReady,
            $"Good news! The health report for {pet.Name} is ready."
        );

        await _notificationRepository.AddAsync(notif, cancellationToken);
        await _notificationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
