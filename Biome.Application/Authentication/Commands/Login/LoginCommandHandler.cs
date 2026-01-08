namespace Biome.Application.Authentication.Commands.Login;

using Biome.Application.Authentication.Common;
using Biome.SharedKernel.Abstractions;
using Biome.Domain.Users;
using Biome.SharedKernel.Primitives;
using Biome.SharedKernel.ValueObjects;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
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

        string token = _jwtTokenGenerator.GenerateToken(
            user.Id, 
            user.FirstName.Value, 
            user.LastName.Value, 
            user.Email.Value, 
            user.Role.Name);

        return new AuthenticationResult(user, token);
    }
}
