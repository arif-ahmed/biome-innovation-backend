using Biome.Domain.Users;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Authentication.Commands.Logout;

internal sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public LogoutCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(new Error("User.NotFound", "User not found."));
        }

        // Revoke refresh token
        user.RevokeRefreshToken();
        
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
