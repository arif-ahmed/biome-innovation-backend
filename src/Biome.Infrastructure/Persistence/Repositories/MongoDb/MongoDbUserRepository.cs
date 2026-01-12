using Biome.Domain.Users;
using Biome.Infrastructure.Persistence.MongoDb;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.ValueObjects;
using MongoDB.Driver;

namespace Biome.Infrastructure.Persistence.Repositories.MongoDb;

public class MongoDbUserRepository : MongoDbRepositoryBase<User>, IUserRepository
{
    public IUnitOfWork UnitOfWork => _unitOfWork;

    public MongoDbUserRepository(MongoDbContext context, MongoDbUnitOfWork unitOfWork) 
        : base(context, unitOfWork)
    {
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq("Email", email.Value);
        return await GetWithFilterSingleAsync(filter, cancellationToken);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // This would need to be implemented based on the actual User entity structure
        // For now, we'll return null as the RefreshTokens property doesn't exist
        await Task.CompletedTask;
        return null;
    }

    public override void Add(User user)
    {
        base.Add(user);
    }
}
