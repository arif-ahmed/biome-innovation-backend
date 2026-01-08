using Biome.Application.Authentication.Common;
using Biome.Domain.Users.Enums;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Users.Commands.AddPet;

public sealed record AddPetCommand(
    Guid UserId,
    string Name,
    PetType Type,
    string? Breed,
    DateTime? DateOfBirth) : IRequest<Result>;
