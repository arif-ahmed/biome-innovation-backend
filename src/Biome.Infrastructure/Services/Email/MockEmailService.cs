using Biome.SharedKernel.Abstractions;
using Microsoft.Extensions.Logging;

namespace Biome.Infrastructure.Services.Email;

public sealed class MockEmailService : IEmailService
{
    private readonly ILogger<MockEmailService> _logger;

    public MockEmailService(ILogger<MockEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending email to {To} with subject {Subject}. Body: {Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
