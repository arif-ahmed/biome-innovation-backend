using Biome.Application.Pets.Common;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Pets.Queries.GetMyPets;

public sealed record GetMyPetsQuery(Guid OwnerId) : IRequest<Result<List<PetResponse>>>;
