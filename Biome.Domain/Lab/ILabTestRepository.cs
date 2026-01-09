using Biome.SharedKernel.Abstractions;

namespace Biome.Domain.Lab;

public interface ILabTestRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task<LabTest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LabTest?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task AddAsync(LabTest labTest, CancellationToken cancellationToken = default);
    Task UpdateAsync(LabTest labTest, CancellationToken cancellationToken = default);
}
