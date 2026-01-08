using Biome.Domain.Users.Enums;
using Biome.SharedKernel.Core;
using Biome.SharedKernel.Primitives;

namespace Biome.Domain.Pets;

public sealed class Pet : AggregateRoot
{
    private Pet(Guid id, Guid ownerId, string name, PetType type, string? breed, DateTime? dateOfBirth)
        : base(id)
    {
        OwnerId = ownerId;
        Name = name;
        Type = type;
        Breed = breed;
        DateOfBirth = dateOfBirth;
    }

    public Guid OwnerId { get; private set; }
    public string Name { get; private set; }
    public PetType Type { get; private set; }
    public string? Breed { get; private set; }
    public DateTime? DateOfBirth { get; private set; }

    public static Result<Pet> Create(Guid ownerId, string name, PetType type, string? breed, DateTime? dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Pet>(new Error("Pet.EmptyName", "Pet name cannot be empty."));
        }

        return new Pet(Guid.NewGuid(), ownerId, name, type, breed, dateOfBirth);
    }
}
