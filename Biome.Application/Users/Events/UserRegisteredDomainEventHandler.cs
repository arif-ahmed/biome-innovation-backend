namespace Biome.Application.Users.Events;

using Biome.Domain.Users.Events;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class UserRegisteredDomainEventHandler : INotificationHandler<UserRegisteredDomainEvent>
{
    private readonly ILogger<UserRegisteredDomainEventHandler> _logger;

    public UserRegisteredDomainEventHandler(ILogger<UserRegisteredDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: Sending activation email to User {UserId}", notification.UserId);
        
        // Simulation of sending email
        return Task.CompletedTask;
    }
}
