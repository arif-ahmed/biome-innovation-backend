using Biome.Domain.Pets;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Primitives;

namespace Biome.Domain.Pets;

public interface IPetRepository
{
    void Add(Pet pet);
    Task<List<Pet>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken);
    Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    IUnitOfWork UnitOfWork { get; }
}
