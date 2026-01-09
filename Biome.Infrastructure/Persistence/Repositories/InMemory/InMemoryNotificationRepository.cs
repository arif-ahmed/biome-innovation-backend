using Biome.Domain.Notifications;
using Biome.SharedKernel.Abstractions;
using MediatR;

namespace Biome.Infrastructure.Persistence.Repositories;

public sealed class InMemoryNotificationRepository : INotificationRepository, IUnitOfWork
{
    private static readonly List<Notification> _notifications = new();
    private readonly IPublisher _publisher;

    public InMemoryNotificationRepository(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public IUnitOfWork UnitOfWork => this;

    public Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_notifications.FirstOrDefault(x => x.Id == id));
    }

    public Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _notifications.Add(notification);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
         var entitiesWithEvents = _notifications.Where(e => e.GetDomainEvents().Any()).ToList();
         var events = entitiesWithEvents.SelectMany(e => e.GetDomainEvents()).ToList();

         foreach (var entity in entitiesWithEvents)
         {
             entity.ClearDomainEvents();
         }

         foreach (var domainEvent in events)
         {
             await _publisher.Publish(domainEvent, cancellationToken);
         }

         return events.Count;
    }
}
