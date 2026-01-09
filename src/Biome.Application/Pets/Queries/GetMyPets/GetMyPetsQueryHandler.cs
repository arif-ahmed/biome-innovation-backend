using Biome.Application.Pets.Common;
using Biome.Domain.Pets;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Pets.Queries.GetMyPets;

internal sealed class GetMyPetsQueryHandler : IRequestHandler<GetMyPetsQuery, Result<List<PetResponse>>>
{
    private readonly IPetRepository _petRepository;

    public GetMyPetsQueryHandler(IPetRepository petRepository)
    {
        _petRepository = petRepository;
    }

    public async Task<Result<List<PetResponse>>> Handle(GetMyPetsQuery request, CancellationToken cancellationToken)
    {
        var pets = await _petRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);

        var response = pets.Select(p => new PetResponse(
            p.Id,
            p.Name,
            p.Type,
            p.Breed,
            p.DateOfBirth)).ToList();

        return response;
    }
}
