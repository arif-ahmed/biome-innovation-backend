using Amazon.DynamoDBv2.DataModel;
using Biome.Domain.Support;
using Biome.SharedKernel.Abstractions;
using MediatR;

namespace Biome.Infrastructure.Persistence.Repositories.DynamoDb;

public class DynamoDbTicketRepository : ITicketRepository, IUnitOfWork
{
    private readonly IDynamoDBContext _context;
    private readonly IPublisher _publisher;

    public DynamoDbTicketRepository(IDynamoDBContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public IUnitOfWork UnitOfWork => this;

    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        await _context.SaveAsync(ticket, cancellationToken);
    }

    public async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LoadAsync<Ticket>(id, cancellationToken);
    }

    public async Task<List<Ticket>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var conditions = new List<ScanCondition>
        {
            new ScanCondition(nameof(Ticket.CustomerId), Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, customerId)
        };
        return await _context.ScanAsync<Ticket>(conditions).GetRemainingAsync(cancellationToken);
    }

    public async Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        await _context.SaveAsync(ticket, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Implementation note: 
        // In a real DynamoDB + Domain Events scenario, we would use the Outbox pattern 
        // or ensure events are published after successful write.
        // Here we are stubbing to meet the interface requirements.
        return 1;
    }
}
