namespace Biome.Api.Controllers;

using Biome.Application.Authentication.Commands.Login;
using Biome.Application.Authentication.Common;
using Biome.SharedKernel.Primitives;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Generates a new 2FA secret key for the authenticated user.
    /// </summary>
    /// <returns>The secret key.</returns>
    [HttpPost("2fa/generate")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateTwoFactorSecret()
    {
        var userId = GetUserId();
        var query = new Biome.Application.Authentication.Queries.GenerateTwoFactorSecret.GenerateTwoFactorSecretQuery(userId);
        Result<string> result = await _sender.Send(query);

        return Ok(new { Secret = result.Value });
    }

    /// <summary>
    /// Enables 2FA for the user by verifying a code against the secret.
    /// </summary>
    /// <param name="request">Secret and Code.</param>
    /// <returns>Status 200 OK.</returns>
    [HttpPost("2fa/enable")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EnableTwoFactor([FromBody] EnableTwoFactorRequest request)
    {
        var userId = GetUserId();
        var command = new Biome.Application.Authentication.Commands.EnableTwoFactor.EnableTwoFactorCommand(userId, request.Secret, request.Code);
        Result result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Completes the login process using a 2FA code.
    /// </summary>
    /// <param name="command">Email and Code.</param>
    /// <returns>Authentication Result (Token).</returns>
    [HttpPost("login-2fa")]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginTwoFactor([FromBody] Biome.Application.Authentication.Commands.LoginTwoFactor.LoginTwoFactorCommand command)
    {
        Result<AuthenticationResult> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return Unauthorized(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        Result<AuthenticationResult> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return Unauthorized(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] Biome.Application.Authentication.Commands.RefreshToken.RefreshTokenCommand command)
    {
        Result<AuthenticationResult> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] Biome.Application.Authentication.Commands.ForgotPassword.ForgotPasswordCommand command)
    {
        Result result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] Biome.Application.Authentication.Commands.ResetPassword.ResetPasswordCommand command)
    {
        Result result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("logout")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> Logout()
    {
        // Extract UserId from Claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var command = new Biome.Application.Authentication.Commands.Logout.LogoutCommand(userId);
        Result result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

public record EnableTwoFactorRequest(string Secret, string Code);
