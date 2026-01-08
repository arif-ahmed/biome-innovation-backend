using Biome.Domain.Payments;
using Biome.Domain.Payments.Events;
using Biome.SharedKernel.Abstractions;
using MediatR;

namespace Biome.Infrastructure.Persistence.Repositories;

public class InMemoryPaymentRepository : IPaymentRepository, IUnitOfWork
{
    private static readonly List<Payment> _payments = new();
    private readonly IPublisher _publisher;

    public InMemoryPaymentRepository(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public IUnitOfWork UnitOfWork => this;

    public void Add(Payment payment)
    {
        _payments.Add(payment);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var paymentsWithEvents = _payments.Where(p => p.GetDomainEvents().Any()).ToList();

        var domainEvents = paymentsWithEvents
            .SelectMany(p => p.GetDomainEvents())
            .ToList();

        foreach (var payment in paymentsWithEvents)
        {
            payment.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        return domainEvents.Count;
    }
}
