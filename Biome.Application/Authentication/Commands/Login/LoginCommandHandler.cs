namespace Biome.Application.Authentication.Commands.Login;

using System.Linq;
using Biome.Application.Authentication.Common;
using Biome.SharedKernel.Abstractions;
using Biome.Domain.Roles;
using Biome.Domain.Users;
using Biome.SharedKernel.Primitives;
using Biome.SharedKernel.ValueObjects;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<AuthenticationResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Result<Email> emailResult = Email.Create(request.Email);
        
        if (emailResult.IsFailure)
        {
            return Result.Failure<AuthenticationResult>(new Error("User.InvalidEmail", "Invalid email format."));
        }

        User? user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (user is null)
        {
            return Result.Failure<AuthenticationResult>(new Error("User.InvalidCredentials", "Invalid email or password."));
        }

        // Domain Rule Check (e.g. Banned)
        Result eligibilityResult = user.EnsureLoginEligibility();
        if (eligibilityResult.IsFailure)
        {
            return Result.Failure<AuthenticationResult>(eligibilityResult.Error);
        }

        bool verified = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!verified)
        {
            return Result.Failure<AuthenticationResult>(new Error("User.InvalidCredentials", "Invalid email or password."));
        }

        // Feature: RBAC - User now holds a RoleId, not the Role object directly (DDD).
        // We must fetch the Role to get its Name for the Token claim.
        // In a Production app, this might be cached or eagerly loaded if EF Core includes navigation properties.
        var role = await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
        
        // Fallback or Error if Role is missing (Invariant violation technically)
        string roleName = role?.Name ?? "Unknown";
        var permissions = role?.Permissions.Select(p => p.Code).ToList() ?? new List<string>();

        string token = _jwtTokenGenerator.GenerateToken(
            user.Id, 
            user.FirstName.Value, 
            user.LastName.Value, 
            user.Email.Value, 
            roleName,
            permissions);

        return new AuthenticationResult(user, token);
    }
}
