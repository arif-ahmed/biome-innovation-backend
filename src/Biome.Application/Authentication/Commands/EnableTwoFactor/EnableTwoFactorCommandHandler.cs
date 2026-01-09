using Biome.SharedKernel.Abstractions;
using Biome.Domain.Users;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Authentication.Commands.EnableTwoFactor;

internal sealed class EnableTwoFactorCommandHandler : IRequestHandler<EnableTwoFactorCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ITwoFactorService _twoFactorService;

    public EnableTwoFactorCommandHandler(IUserRepository userRepository, ITwoFactorService twoFactorService)
    {
        _userRepository = userRepository;
        _twoFactorService = twoFactorService;
    }

    public async Task<Result> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
             return Result.Failure(new Error("User.NotFound", "User not found."));
        }

        var result = user.EnableTwoFactor(request.Secret, request.Code, _twoFactorService);
        if (result.IsFailure)
        {
            return result;
        }

        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
