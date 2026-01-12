using Biome.SharedKernel.Abstractions;

namespace Biome.Infrastructure.Persistence.MongoDb;

public class MongoDbUnitOfWork : IUnitOfWork
{
    private readonly MongoDbContext _context;
    private readonly List<Func<Task>> _operations;

    public MongoDbUnitOfWork(MongoDbContext context)
    {
        _context = context;
        _operations = new List<Func<Task>>();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // MongoDB doesn't have transactions in the same way as relational databases
        // Each operation is atomic at the document level
        // For multi-document transactions, you would need to use MongoDB's session transactions
        // This is a simplified implementation that just executes all pending operations
        
        var tasks = _operations.Select(op => op()).ToArray();
        Task.WaitAll(tasks, cancellationToken);
        
        _operations.Clear();
        return Task.FromResult(tasks.Length);
    }

    public void RegisterOperation(Func<Task> operation)
    {
        _operations.Add(operation);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
