using Biome.Application.Authentication.Common;
using Biome.SharedKernel.Abstractions;
using Biome.Domain.Users;
using Biome.SharedKernel.Primitives;
using MediatR;
using Biome.Domain.Roles;

namespace Biome.Application.Authentication.Commands.LoginTwoFactor;

internal sealed class LoginTwoFactorCommandHandler : IRequestHandler<LoginTwoFactorCommand, Result<AuthenticationResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ITwoFactorService _twoFactorService;
    private readonly IRoleRepository _roleRepository;

    public LoginTwoFactorCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        ITwoFactorService twoFactorService,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _twoFactorService = twoFactorService;
        _roleRepository = roleRepository;
    }

    public async Task<Result<AuthenticationResult>> Handle(LoginTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Biome.SharedKernel.ValueObjects.Email.Create(request.Email);
        if (emailResult.IsFailure) return Result.Failure<AuthenticationResult>(emailResult.Error);

        var user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null)
        {
             return Result.Failure<AuthenticationResult>(new Error("User.InvalidCredentials", "Invalid email or code."));
        }

        var verificationResult = user.VerifyTwoFactorLogin(request.Code, _twoFactorService);
        if (verificationResult.IsFailure)
        {
             return Result.Failure<AuthenticationResult>(verificationResult.Error);
        }

        // Generate Tokens (Copy logic from LoginCommandHandler - DRY later)
        var role = await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
        string roleName = role?.Name ?? "Unknown";
        var permissions = role?.Permissions.Select(p => p.Code).ToList() ?? new List<string>();

        string token = _jwtTokenGenerator.GenerateToken(
            user.Id, 
            user.FirstName.Value, 
            user.LastName.Value, 
            user.Email.Value, 
            roleName,
            permissions);

        string refreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();
        var refreshToken = Biome.Domain.Users.ValueObjects.RefreshToken.Create(refreshTokenValue, DateTime.UtcNow.AddDays(7));
        user.SetRefreshToken(refreshToken);

        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(user, token, refreshTokenValue);
    }
}
