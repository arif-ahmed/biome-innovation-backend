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
}
