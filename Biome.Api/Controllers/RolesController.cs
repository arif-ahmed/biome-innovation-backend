namespace Biome.Api.Controllers;

using Biome.Application.Roles.Commands.CreateRole;
using Biome.SharedKernel.Primitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/roles")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly ISender _sender;

    public RolesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Policy = "Roles:Create")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
    {
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("permissions")]
    [Authorize(Policy = "Roles:Read")]
    public async Task<IActionResult> GetPermissions()
    {
        var query = new Biome.Application.Roles.Queries.GetPermissions.GetPermissionsQuery();
        var result = await _sender.Send(query);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("{id}/permissions")]
    [Authorize(Policy = "Roles:Update")]
    public async Task<IActionResult> AssignPermissions(Guid id, [FromBody] List<string> permissions)
    {
        var command = new Biome.Application.Roles.Commands.AssignPermissions.AssignPermissionsToRoleCommand(id, permissions);
        var result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok();
    }
}
