namespace Biome.Api.Controllers;

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
}
