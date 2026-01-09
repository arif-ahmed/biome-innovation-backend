using Biome.Domain.Pets;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Pets.Commands.CreatePet;

internal sealed class CreatePetCommandHandler : IRequestHandler<CreatePetCommand, Result<Guid>>
{
    private readonly IPetRepository _petRepository;

    public CreatePetCommandHandler(IPetRepository petRepository)
    {
        _petRepository = petRepository;
    }

    public async Task<Result<Guid>> Handle(CreatePetCommand request, CancellationToken cancellationToken)
    {
        var petResult = Pet.Create(request.OwnerId, request.Name, request.Type, request.Breed, request.DateOfBirth);

        if (petResult.IsFailure)
        {
            return Result.Failure<Guid>(petResult.Error);
        }

        _petRepository.Add(petResult.Value);
        await _petRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return petResult.Value.Id;
    }
}
