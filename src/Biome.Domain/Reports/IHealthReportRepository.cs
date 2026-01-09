using Biome.SharedKernel.Abstractions;

namespace Biome.Domain.Reports;

public interface IHealthReportRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task<HealthReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<HealthReport?> GetByLabTestIdAsync(Guid labTestId, CancellationToken cancellationToken = default);
    Task AddAsync(HealthReport report, CancellationToken cancellationToken = default);
}
