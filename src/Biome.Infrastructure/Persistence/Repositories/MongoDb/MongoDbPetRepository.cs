using Biome.Domain.Pets;
using Biome.Infrastructure.Persistence.MongoDb;
using Biome.SharedKernel.Abstractions;
using MongoDB.Driver;

namespace Biome.Infrastructure.Persistence.Repositories.MongoDb;

public class MongoDbPetRepository : MongoDbRepositoryBase<Pet>, IPetRepository
{
    public IUnitOfWork UnitOfWork => _unitOfWork;

    public MongoDbPetRepository(MongoDbContext context, MongoDbUnitOfWork unitOfWork) 
        : base(context, unitOfWork)
    {
    }

    public async Task<List<Pet>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        var filter = Builders<Pet>.Filter.Eq("OwnerId", ownerId);
        var pets = await GetWithFilterAsync(filter, cancellationToken);
        return pets.ToList();
    }
}
