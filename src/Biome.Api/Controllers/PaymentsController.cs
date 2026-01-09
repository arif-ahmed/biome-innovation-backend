namespace Biome.Api.Controllers;

using Biome.Application.Payments.Commands.ProcessPayment;
using Biome.SharedKernel.Primitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/v1/payments")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly ISender _sender;

    public PaymentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        var command = new ProcessPaymentCommand(request.OrderId, request.Amount, request.Currency, request.Token);
        Result result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
}

public record ProcessPaymentRequest(Guid OrderId, decimal Amount, string Currency, string Token);
