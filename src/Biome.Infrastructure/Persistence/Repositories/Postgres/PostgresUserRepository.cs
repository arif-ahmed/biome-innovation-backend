using Biome.Domain.Users;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Biome.Infrastructure.Persistence.Repositories.Postgres;

public class PostgresUserRepository : IUserRepository
{
    private readonly BiomeDbContext _context;

    public PostgresUserRepository(BiomeDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context; // BiomeDbContext must implement IUnitOfWork

    public void Add(User user)
    {
        _context.Users.Add(user);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .SingleOrDefaultAsync(u => u.RefreshToken != null && u.RefreshToken.Token == refreshToken, cancellationToken);
    }
}
