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

    [HttpPost]
    [Authorize]
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

    [HttpGet]
    [Authorize]
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
    
    [HttpPost("{id}/pay")]
    [Authorize]
    public async Task<IActionResult> PayOrder(Guid id)
    {
        var command = new Biome.Application.Orders.Commands.PayOrder.PayOrderCommand(id);
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
