using Biome.Domain.Users.Enums;

namespace Biome.Application.Pets.Common;

public sealed record PetResponse(Guid Id, string Name, PetType Type, string? Breed, DateTime? DateOfBirth);
