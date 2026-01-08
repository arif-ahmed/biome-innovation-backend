using Biome.Domain.Users;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Users.Commands.AddPet;

internal sealed class AddPetCommandHandler : IRequestHandler<AddPetCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public AddPetCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(AddPetCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(new Error("User.NotFound", "User not found."));
        }

        Result result = user.AddPet(request.Name, request.Type, request.Breed, request.DateOfBirth);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
