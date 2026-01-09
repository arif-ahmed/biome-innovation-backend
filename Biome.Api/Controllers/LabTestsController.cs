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

    /// <summary>
    /// Records the results of a lab test for a specific order.
    /// </summary>
    /// <param name="request">The test results data.</param>
    /// <returns>Status 200 OK if successful.</returns>
    [HttpPost("record-results")]
    [Authorize] // Should be restricted to Lab Staff role in future
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Retrieves a lab test associated with a specific order ID.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <returns>The lab test details.</returns>
    [HttpGet("order/{orderId}")]
    [Authorize]
    [ProducesResponseType(typeof(LabTestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
