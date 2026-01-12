using Biome.Domain.Support;
using Biome.Infrastructure.Persistence.MongoDb;
using Biome.SharedKernel.Abstractions;
using MongoDB.Driver;

namespace Biome.Infrastructure.Persistence.Repositories.MongoDb;

public class MongoDbTicketRepository : MongoDbRepositoryBase<Ticket>, ITicketRepository
{
    public IUnitOfWork UnitOfWork => _unitOfWork;

    public MongoDbTicketRepository(MongoDbContext context, MongoDbUnitOfWork unitOfWork) 
        : base(context, unitOfWork)
    {
    }

    public async Task<List<Ticket>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Ticket>.Filter.Eq("CustomerId", customerId);
        var tickets = await GetWithFilterAsync(filter, cancellationToken);
        return tickets.ToList();
    }

    public Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        Add(ticket);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        Update(ticket);
        return Task.CompletedTask;
    }
}
