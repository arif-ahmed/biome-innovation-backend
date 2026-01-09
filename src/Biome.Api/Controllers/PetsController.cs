namespace Biome.Api.Controllers;

using Biome.Application.Pets.Commands.CreatePet;
using Biome.Application.Pets.Queries.GetMyPets;
using Biome.Domain.Users.Enums;
using Biome.SharedKernel.Primitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/v1/pets")]
[ApiController]
public class PetsController : ControllerBase
{
    private readonly ISender _sender;

    public PetsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePet([FromBody] CreatePetRequest request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var command = new CreatePetCommand(userId, request.Name, request.Type, request.Breed, request.DateOfBirth);
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMyPets()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var query = new GetMyPetsQuery(userId);
        Result<List<Biome.Application.Pets.Common.PetResponse>> result = await _sender.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

public record CreatePetRequest(string Name, PetType Type, string? Breed, DateTime? DateOfBirth);
