namespace Biome.Domain.Users;

using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.ValueObjects;

public interface IUserRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    void Add(User user);
}
