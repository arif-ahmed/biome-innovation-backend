using Biome.Application.Authentication.Common;
using Biome.Domain.Roles;
using Biome.Domain.Users;
using Biome.Domain.Users.ValueObjects;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Authentication.Commands.RefreshToken;

internal sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRoleRepository _roleRepository;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _roleRepository = roleRepository;
    }

    public async Task<Result<AuthenticationResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user is null || user.RefreshToken is null || user.RefreshToken.Token != request.RefreshToken)
        {
            return Result.Failure<AuthenticationResult>(new Error("RefreshToken.Invalid", "The refresh token is invalid."));
        }

        if (user.RefreshToken.ExpiryOnUtc < DateTime.UtcNow)
        {
            return Result.Failure<AuthenticationResult>(new Error("RefreshToken.Expired", "The refresh token has expired."));
        }

        var role = await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
        if (role is null)
        {
             return Result.Failure<AuthenticationResult>(new Error("Role.NotFound", "User role not found."));
        }

        string accessToken = _jwtTokenGenerator.GenerateToken(
            user.Id,
            user.FirstName.Value,
            user.LastName.Value,
            user.Email.Value,
            role.Name,
            role.Permissions.Select(p => p.Code));

        string newRefreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();
        var newRefreshToken = Biome.Domain.Users.ValueObjects.RefreshToken.Create(newRefreshTokenValue, DateTime.UtcNow.AddDays(7));
        
        user.SetRefreshToken(newRefreshToken);

        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(user, accessToken, newRefreshTokenValue);
    }
}
