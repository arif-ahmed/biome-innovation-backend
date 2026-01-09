using Biome.Domain.Reports;
using Biome.SharedKernel.Abstractions;
using MediatR;

namespace Biome.Infrastructure.Persistence.Repositories;

public sealed class InMemoryHealthReportRepository : IHealthReportRepository, IUnitOfWork
{
    private static readonly List<HealthReport> _reports = new();
    private readonly IPublisher _publisher;

    public InMemoryHealthReportRepository(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public IUnitOfWork UnitOfWork => this;

    public Task<HealthReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
         return Task.FromResult(_reports.FirstOrDefault(x => x.Id == id));
    }

    public Task<HealthReport?> GetByLabTestIdAsync(Guid labTestId, CancellationToken cancellationToken = default)
    {
         return Task.FromResult(_reports.FirstOrDefault(x => x.LabTestId == labTestId));
    }

    public Task AddAsync(HealthReport report, CancellationToken cancellationToken = default)
    {
        _reports.Add(report);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
         var entitiesWithEvents = _reports.Where(e => e.GetDomainEvents().Any()).ToList();
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
