namespace Biome.Api.Controllers;

using Biome.Application.Orders.Commands.CreateOrder;
using Biome.SharedKernel.Primitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/v1/orders")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a new order for the authenticated user.
    /// </summary>
    /// <param name="items">The list of items to order.</param>
    /// <returns>The unique identifier of the created order.</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateOrder([FromBody] List<OrderItemDto> items)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var command = new CreateOrderCommand(userId, items);
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Retrieves all orders placed by the authenticated user.
    /// </summary>
    /// <returns>A list of orders.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)] // Can be more specific if DTO exists
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var query = new Biome.Application.Orders.Queries.GetMyOrders.GetMyOrdersQuery(userId);
        var result = await _sender.Send(query);

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
