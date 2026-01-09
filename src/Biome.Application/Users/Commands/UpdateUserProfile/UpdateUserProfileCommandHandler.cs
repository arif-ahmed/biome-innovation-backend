using Biome.Domain.Users;
using Biome.Domain.Users.ValueObjects;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Users.Commands.UpdateUserProfile;

internal sealed class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserProfileCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(new Error("User.NotFound", "User not found."));
        }

        Result<FirstName> firstNameResult = FirstName.Create(request.FirstName);
        Result<LastName> lastNameResult = LastName.Create(request.LastName);

        var validationResult = Result.Combine(firstNameResult, lastNameResult);
        if (validationResult.IsFailure)
        {
            return Result.Failure(validationResult.Error);
        }

        user.UpdateProfile(firstNameResult.Value, lastNameResult.Value);

        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
