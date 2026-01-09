using Biome.Domain.Users;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Users.Commands.ChangePassword;

internal sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(new Error("User.NotFound", "User not found."));
        }

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return Result.Failure(new Error("User.InvalidPassword", "The current password is incorrect."));
        }

        string newPasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.ChangePassword(newPasswordHash);

        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
