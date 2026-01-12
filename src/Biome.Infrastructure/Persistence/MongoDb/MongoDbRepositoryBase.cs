using MongoDB.Driver;
using MongoDB.Bson;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Core;

namespace Biome.Infrastructure.Persistence.MongoDb;

public abstract class MongoDbRepositoryBase<T> where T : class, IEntity
{
    protected readonly MongoDbContext _context;
    protected readonly IMongoCollection<T> _collection;
    protected readonly MongoDbUnitOfWork _unitOfWork;

    protected MongoDbRepositoryBase(MongoDbContext context, MongoDbUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _collection = _context.GetCollection<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>.Filter.Eq("_id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.Find(_ => true).ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetWithFilterAsync(
        FilterDefinition<T> filter,
        CancellationToken cancellationToken = default)
    {
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> GetWithFilterSingleAsync(
        FilterDefinition<T> filter,
        CancellationToken cancellationToken = default)
    {
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual void Add(T entity)
    {
        _unitOfWork.RegisterOperation(async () =>
        {
            await _collection.InsertOneAsync(entity);
        });
    }

    public virtual void Update(T entity)
    {
        _unitOfWork.RegisterOperation(async () =>
        {
            var filter = Builders<T>.Filter.Eq("_id", ((Entity)(object)entity).Id);
            await _collection.ReplaceOneAsync(filter, entity);
        });
    }

    public virtual void Delete(Guid id)
    {
        _unitOfWork.RegisterOperation(async () =>
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        });
    }

    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>.Filter.Eq("_id", id);
        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }

    public virtual async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(FilterDefinition<T>.Empty, cancellationToken: cancellationToken);
    }

    public virtual async Task<long> CountAsync(FilterDefinition<T> filter, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}
