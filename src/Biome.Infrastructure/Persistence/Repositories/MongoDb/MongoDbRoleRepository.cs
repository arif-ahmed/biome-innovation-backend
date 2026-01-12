using Biome.Domain.Roles;
using Biome.Infrastructure.Persistence.MongoDb;
using Biome.SharedKernel.Abstractions;
using MongoDB.Driver;

namespace Biome.Infrastructure.Persistence.Repositories.MongoDb;

public class MongoDbRoleRepository : MongoDbRepositoryBase<Role>, IRoleRepository
{
    public IUnitOfWork UnitOfWork => _unitOfWork;

    public MongoDbRoleRepository(MongoDbContext context, MongoDbUnitOfWork unitOfWork) 
        : base(context, unitOfWork)
    {
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Role>.Filter.Eq("Name", name);
        return await GetWithFilterSingleAsync(filter, cancellationToken);
    }
}
