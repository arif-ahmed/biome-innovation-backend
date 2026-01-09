using Biome.Application.Shipping.Commands.CreateShipment;
using Biome.SharedKernel.Enums;
using Biome.SharedKernel.Primitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biome.Api.Controllers;

[Route("api/v1/shipments")]
[ApiController]
public class ShippingController : ControllerBase
{
    private readonly ISender _sender;

    public ShippingController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a shipment and generates a label for an order.
    /// </summary>
    /// <param name="request">Order ID and Carrier details.</param>
    /// <returns>The ID of the created shipment.</returns>
    [HttpPost]
    [Authorize] // Should be Admin/Warehouse only
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentRequest request)
    {
        var command = new CreateShipmentCommand(request.OrderId, request.Carrier, request.Address);
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}

public record CreateShipmentRequest(Guid OrderId, Carrier Carrier, string Address);
