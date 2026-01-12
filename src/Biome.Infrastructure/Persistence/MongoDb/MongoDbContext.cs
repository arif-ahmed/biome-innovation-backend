using Biome.Infrastructure.Persistence.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Biome.Infrastructure.Persistence.MongoDb;

public class MongoDbContext : IDisposable
{
    private readonly IMongoDatabase _database;
    private bool _disposed;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var mongoSettings = settings.Value;
        var client = new MongoClient(mongoSettings.ConnectionString);
        _database = client.GetDatabase(mongoSettings.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string? name = null)
    {
        var collectionName = name ?? typeof(T).Name;
        return _database.GetCollection<T>(collectionName);
    }

    public IMongoDatabase Database => _database;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // MongoDB client is managed by the driver, no explicit disposal needed
        }
        _disposed = true;
    }
}
