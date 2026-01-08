namespace Biome.Application.Users.Commands.CreateUser;

using Biome.SharedKernel.Abstractions;
using Biome.Domain.Users;
using Biome.Domain.Users.ValueObjects;
using Biome.SharedKernel.Primitives;
using Biome.SharedKernel.ValueObjects;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        Result<Email> emailResult = Email.Create(request.Email);
        Result<FirstName> firstNameResult = FirstName.Create(request.FirstName);
        Result<LastName> lastNameResult = LastName.Create(request.LastName);

        // Basic validation result checks
        if (emailResult.IsFailure) return Result.Failure<Guid>(emailResult.Error);
        if (firstNameResult.IsFailure) return Result.Failure<Guid>(firstNameResult.Error);
        if (lastNameResult.IsFailure) return Result.Failure<Guid>(lastNameResult.Error);

        // Check for uniqueness
        if (await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken) is not null)
        {
            return Result.Failure<Guid>(new Error("User.EmailAlreadyExists", "The email is already in use."));
        }

        // Parse Role (Validator ensures it's valid)
        if (!UserRole.TryFromName(request.Role, true, out var role) || role is null)
        {
             return Result.Failure<Guid>(new Error("User.InvalidRole", "Invalid role specified."));
        }

        string passwordHash = _passwordHasher.Hash(request.Password);

        // Use the Admin-specific Create factory which triggers UserCreatedDomainEvent
        // We pass the raw password as "temporary password" for the event payload, 
        // so the email handler can send it to the user.
        var user = User.Create(
            firstNameResult.Value,
            lastNameResult.Value,
            emailResult.Value,
            passwordHash,
            role!,
            request.Password); 

        _userRepository.Add(user);
        
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
