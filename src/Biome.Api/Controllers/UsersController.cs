namespace Biome.Api.Controllers;

using Biome.Application.Users.Commands.CreateUser;
using Biome.Application.Users.Commands.RegisterUser;
using Biome.SharedKernel.Primitives;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("api/v1/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Policy = "Users:Create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> GetMe()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var query = new Biome.Application.Users.Queries.GetUserProfile.GetUserProfileQuery(userId);
        var result = await _sender.Send(query);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPut("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserProfileRequest request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var command = new Biome.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileCommand(userId, request.FirstName, request.LastName);
        var result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok();
    }

    [HttpPut("me/password")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var command = new Biome.Application.Users.Commands.ChangePassword.ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword);
        var result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok();
    }


    private Guid GetUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

public record UpdateUserProfileRequest(string FirstName, string LastName);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
