using Biome.Application.Lab.Commands.RecordTestResults;
using Biome.Application.Lab.Queries.GetLabTestByOrder;
using Biome.SharedKernel.Primitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biome.Api.Controllers;

[Route("api/v1/lab-tests")]
[ApiController]
public class LabTestsController : ControllerBase
{
    private readonly ISender _sender;

    public LabTestsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("record-results")]
    [Authorize] // Should be restricted to Lab Staff role in future
    public async Task<IActionResult> RecordResults([FromBody] RecordLabTestResultsRequest request)
    {
        var command = new RecordLabTestResultsCommand(request.OrderId, request.RawDataJson);
        Result result = await _sender.Send(command);

        if (result.IsFailure)
        {
             return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet("order/{orderId}")]
    [Authorize]
    public async Task<IActionResult> GetByOrder(Guid orderId)
    {
        var query = new GetLabTestByOrderIdQuery(orderId);
        Result<LabTestDto> result = await _sender.Send(query);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }
}

public record RecordLabTestResultsRequest(Guid OrderId, string RawDataJson);
