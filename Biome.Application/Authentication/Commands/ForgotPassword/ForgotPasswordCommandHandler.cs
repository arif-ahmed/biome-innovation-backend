using Biome.Domain.Users;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Primitives;
using Biome.SharedKernel.ValueObjects;
using MediatR;

namespace Biome.Application.Authentication.Commands.ForgotPassword;

internal sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        Result<Email> emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure) return Result.Failure(emailResult.Error);

        var user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        
        // Don't reveal if user exists or not for security, but logic proceeds if user is found.
        if (user is not null)
        {
            // Generate simple token (GUID or random string)
            string token = Guid.NewGuid().ToString();
            user.RequestPasswordReset(token, DateTime.UtcNow.AddHours(1));

            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            await _emailService.SendEmailAsync(
                user.Email.Value, 
                "Password Reset Request", 
                $"Your password reset token is: {token}");
        }

        return Result.Success();
    }
}
