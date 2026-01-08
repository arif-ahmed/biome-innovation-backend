namespace Biome.Infrastructure.Services;

using Biome.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Sending email to {To} with subject {Subject}: {Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
