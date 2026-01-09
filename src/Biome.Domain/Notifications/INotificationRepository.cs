using Biome.SharedKernel.Abstractions;

namespace Biome.Domain.Notifications;

public interface INotificationRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
}
