using Biome.Domain.Support;
using Biome.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Biome.Infrastructure.Persistence.Repositories.Postgres;

public class PostgresTicketRepository : ITicketRepository
{
    private readonly BiomeDbContext _context;

    public PostgresTicketRepository(BiomeDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        await _context.Tickets.AddAsync(ticket, cancellationToken);
    }

    public async Task<List<Ticket>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Tickets
            .Where(t => t.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        // EF Core tracks changes, so no explicit update needed if retrieved from context.
        // But if passed detached, we explicitly attach.
        // Usually, in Clean Architecture command handlers, we fetch, modify, save.
        // So this might just be a no-op or EnsureAttached.
        if (_context.Entry(ticket).State == EntityState.Detached)
        {
            _context.Tickets.Update(ticket);
        }
        
        return Task.CompletedTask;
    }
}
