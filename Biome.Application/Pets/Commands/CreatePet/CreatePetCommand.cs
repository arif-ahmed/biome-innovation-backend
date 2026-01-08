using Biome.Domain.Users.Enums;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Pets.Commands.CreatePet;

public sealed record CreatePetCommand(Guid OwnerId, string Name, PetType Type, string? Breed, DateTime? DateOfBirth) : IRequest<Result<Guid>>;
