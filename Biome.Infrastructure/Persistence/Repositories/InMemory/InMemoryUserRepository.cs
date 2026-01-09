namespace Biome.Infrastructure.Persistence.Repositories;

using Biome.Domain.Users;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.ValueObjects;
using MediatR;
using System.Linq;

public class InMemoryUserRepository : IUserRepository, IUnitOfWork
{
    private static readonly List<User> _users = new();
    private readonly IPublisher _publisher;

    public InMemoryUserRepository(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public IUnitOfWork UnitOfWork => this;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(u => u.Email == email);
        return Task.FromResult(user);
    }

    public Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(u => u.RefreshToken?.Token == refreshToken);
        return Task.FromResult(user);
    }

    public void Add(User user)
    {
        _users.Add(user);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Get all domain events from all tracked entities (in this case, all users in the list)
        // In a real EF Core implementation, ChangeTracker would give us the modified entities.
        // Here we just scan all users for simplicity.
        var usersWithEvents = _users.Where(u => u.GetDomainEvents().Any()).ToList();

        var domainEvents = usersWithEvents
            .SelectMany(u => u.GetDomainEvents())
            .ToList();

        var result = domainEvents.Count;

        // 2. Clear domain events
        foreach (var user in usersWithEvents)
        {
            user.ClearDomainEvents();
        }

        // 3. Publish domain events
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        return result;
    }
}
