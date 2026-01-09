using Biome.SharedKernel.Abstractions;

namespace Biome.Domain.Support;

public interface ITicketRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Ticket>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default);
}
