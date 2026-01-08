namespace Biome.Application.Users.Events;

using Biome.SharedKernel.Abstractions;
using Biome.Domain.Users;
using Biome.Domain.Users.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public sealed class UserRegisteredDomainEventHandler : INotificationHandler<UserRegisteredDomainEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public UserRegisteredDomainEventHandler(
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);
        if (user is null)
        {
            return;
        }

        await _emailService.SendEmailAsync(
            user.Email.Value,
            "Welcome to Biome Innovation!",
            $"Hi {user.FirstName.Value}, welcome to our platform! Please verify your email.");
    }
}
