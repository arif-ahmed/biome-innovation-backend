using Biome.Domain.Lab;
using Biome.SharedKernel.Abstractions;
using MediatR;

namespace Biome.Infrastructure.Persistence.Repositories;

public sealed class InMemoryLabTestRepository : ILabTestRepository, IUnitOfWork
{
    private static readonly List<LabTest> _labTests = new();
    private readonly IPublisher _publisher;

    public InMemoryLabTestRepository(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public IUnitOfWork UnitOfWork => this;

    public Task<LabTest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_labTests.FirstOrDefault(x => x.Id == id));
    }

    public Task<LabTest?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_labTests.FirstOrDefault(x => x.OrderId == orderId));
    }

    public Task AddAsync(LabTest labTest, CancellationToken cancellationToken = default)
    {
        _labTests.Add(labTest);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(LabTest labTest, CancellationToken cancellationToken = default)
    {
        // In-memory reference is already updated, but explicit update for pattern consistency
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
         var entitiesWithEvents = _labTests.Where(e => e.GetDomainEvents().Any()).ToList();
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
