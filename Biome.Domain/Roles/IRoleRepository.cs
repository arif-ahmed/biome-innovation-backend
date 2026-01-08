namespace Biome.Domain.Roles;

using Biome.SharedKernel.Abstractions;

public interface IRoleRepository
{
    IUnitOfWork UnitOfWork { get; }
    void Add(Role role);
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
