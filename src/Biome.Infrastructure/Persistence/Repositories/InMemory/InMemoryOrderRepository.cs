using Biome.Domain.Orders;
using Biome.SharedKernel.Abstractions;
using MediatR;

namespace Biome.Infrastructure.Persistence.Repositories;

public class InMemoryOrderRepository : IOrderRepository, IUnitOfWork
{
    private static readonly List<Order> _orders = new();
    private readonly IPublisher _publisher;

    public InMemoryOrderRepository(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public IUnitOfWork UnitOfWork => this;

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        return Task.FromResult(order);
    }
    
    public Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var orders = _orders.Where(o => o.CustomerId == customerId).ToList();
        return Task.FromResult(orders);
    }

    public void Add(Order order)
    {
        _orders.Add(order);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var ordersWithEvents = _orders.Where(o => o.GetDomainEvents().Any()).ToList();

        var domainEvents = ordersWithEvents
            .SelectMany(o => o.GetDomainEvents())
            .ToList();

        foreach (var order in ordersWithEvents)
        {
            order.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        return domainEvents.Count;
    }
}
