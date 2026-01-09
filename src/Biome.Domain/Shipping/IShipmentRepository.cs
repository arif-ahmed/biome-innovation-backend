using Biome.SharedKernel.Abstractions;

namespace Biome.Domain.Shipping;

public interface IShipmentRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Shipment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Shipment shipment, CancellationToken cancellationToken = default);
}
