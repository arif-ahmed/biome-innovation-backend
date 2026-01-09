using Biome.Domain.Support;
using Biome.SharedKernel.Abstractions;
using MediatR;

namespace Biome.Infrastructure.Persistence.Repositories;

public sealed class InMemoryTicketRepository : ITicketRepository, IUnitOfWork
{
    private static readonly List<Ticket> _tickets = new();
    private readonly IPublisher _publisher;

    public InMemoryTicketRepository(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public IUnitOfWork UnitOfWork => this;

    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_tickets.FirstOrDefault(t => t.Id == id));
    }

    public Task<List<Ticket>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_tickets.Where(t => t.CustomerId == customerId).OrderByDescending(t => t.LastActivityAt).ToList());
    }

    public Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        _tickets.Add(ticket);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        // Reference type update is implicit in memory
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entitiesWithEvents = _tickets.Where(e => e.GetDomainEvents().Any()).ToList();
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
