using Biome.Domain.Shipping;
using Biome.SharedKernel.Abstractions;
using MediatR;

namespace Biome.Infrastructure.Persistence.Repositories;

public sealed class InMemoryShipmentRepository : IShipmentRepository, IUnitOfWork
{
    private static readonly List<Shipment> _shipments = new();
    private readonly IPublisher _publisher;

    public InMemoryShipmentRepository(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public IUnitOfWork UnitOfWork => this;

    public Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        _shipments.Add(shipment);
        return Task.CompletedTask;
    }

    public Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_shipments.FirstOrDefault(s => s.Id == id));
    }

    public Task<Shipment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_shipments.FirstOrDefault(s => s.OrderId == orderId));
    }

    public Task UpdateAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entitiesWithEvents = _shipments.Where(e => e.GetDomainEvents().Any()).ToList();
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
