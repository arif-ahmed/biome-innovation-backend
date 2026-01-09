using Biome.Domain.Pets;
using Biome.SharedKernel.Abstractions;
using MediatR;

namespace Biome.Infrastructure.Persistence.Repositories;

public class InMemoryPetRepository : IPetRepository, IUnitOfWork
{
    private static readonly List<Pet> _pets = new();
    private readonly IPublisher _publisher;

    public InMemoryPetRepository(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public IUnitOfWork UnitOfWork => this;

    public void Add(Pet pet)
    {
        _pets.Add(pet);
    }

    public Task<List<Pet>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_pets.Where(p => p.OwnerId == ownerId).ToList());
    }

    public Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_pets.FirstOrDefault(p => p.Id == id));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var petsWithEvents = _pets.Where(p => p.GetDomainEvents().Any()).ToList();

        var domainEvents = petsWithEvents
            .SelectMany(p => p.GetDomainEvents())
            .ToList();

        foreach (var pet in petsWithEvents)
        {
            pet.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        return domainEvents.Count;
    }
}
