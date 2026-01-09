using Biome.SharedKernel.Abstractions;

namespace Biome.Infrastructure.Persistence.Repositories.DynamoDb;

public class DynamoDbUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // DynamoDB logic for saving changes (e.g. TransactWriteItems) would go here.
        // For simple single-table operations or immediate writes, this might be a no-op or return 1.
        return Task.FromResult(1);
    }
}
