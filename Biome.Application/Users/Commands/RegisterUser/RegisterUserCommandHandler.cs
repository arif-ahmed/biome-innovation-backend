namespace Biome.Application.Users.Commands.RegisterUser;

using Biome.SharedKernel.Abstractions;
using Biome.Domain.Users;
using Biome.Domain.Users.ValueObjects;
using Biome.SharedKernel.Primitives;
using Biome.SharedKernel.ValueObjects;
using MediatR;

internal sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        Result<Email> emailResult = Email.Create(request.Email);
        Result<FirstName> firstNameResult = FirstName.Create(request.FirstName);
        Result<LastName> lastNameResult = LastName.Create(request.LastName);

        if (emailResult.IsFailure)
        {
            return Result.Failure<Guid>(emailResult.Error);
        }

        if (firstNameResult.IsFailure)
        {
            return Result.Failure<Guid>(firstNameResult.Error);
        }

        if (lastNameResult.IsFailure)
        {
            return Result.Failure<Guid>(lastNameResult.Error);
        }

        if (await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken) is not null)
        {
            return Result.Failure<Guid>(new Error("User.EmailAlreadyExists", "The email is already in use."));
        }

        string passwordHash = _passwordHasher.Hash(request.Password);

        var user = User.Register(
            firstNameResult.Value,
            lastNameResult.Value,
            emailResult.Value,
            passwordHash,
            UserRole.Customer);

        _userRepository.Add(user);
        
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
