using Biome.Domain.Users;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Primitives;
using Biome.SharedKernel.ValueObjects;
using MediatR;

namespace Biome.Application.Authentication.Commands.ResetPassword;

internal sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        Result<Email> emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure) return Result.Failure(emailResult.Error);

        var user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null)
        {
            return Result.Failure(new Error("User.NotFound", "User not found."));
        }

        string passwordHash = _passwordHasher.Hash(request.NewPassword);

        var result = user.ResetPassword(request.Token, passwordHash);
        if (result.IsFailure)
        {
            return result;
        }

        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
