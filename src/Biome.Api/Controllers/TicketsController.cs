using Biome.Application.Support.Commands.AddReply;
using Biome.Application.Support.Commands.CreateTicket;
using Biome.Application.Support.Queries.GetTickets;
using Biome.SharedKernel.Primitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biome.Api.Controllers;

[Route("api/v1/tickets")]
[ApiController]
public class TicketsController : ControllerBase
{
    private readonly ISender _sender;

    public TicketsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a new support ticket.
    /// </summary>
    /// <param name="request">Ticket details.</param>
    /// <returns>The ID of the created ticket.</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
    {
        var userId = GetUserId();
        var command = new CreateTicketCommand(userId, request.Subject, request.Message);
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Adds a reply to an existing ticket.
    /// </summary>
    /// <param name="id">The ticket ID.</param>
    /// <param name="request">Reply content.</param>
    /// <returns>Status 200 OK.</returns>
    [HttpPost("{id}/reply")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReplyToTicket(Guid id, [FromBody] ReplyRequest request)
    {
        var userId = GetUserId();
        // MVP Assumption: If user is calling, it's a customer. In future, check roles.
        var isFromCustomer = true; 

        var command = new AddReplyCommand(id, userId, request.Content, isFromCustomer);
        Result result = await _sender.Send(command);

        if (result.IsFailure)
        {
            if (result.Error.Code == "Ticket.NotFound") return NotFound(result.Error);
            return BadRequest(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Retrieves all tickets for the authenticated user.
    /// </summary>
    /// <returns>List of tickets.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<TicketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTickets()
    {
        var userId = GetUserId();
        var query = new GetCustomerTicketsQuery(userId);
        Result<List<TicketDto>> result = await _sender.Send(query);

        return Ok(result.Value);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

public record CreateTicketRequest(string Subject, string Message);
public record ReplyRequest(string Content);
