using Biome.Application.Reports.Queries.GetReportById;
using Biome.SharedKernel.Primitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biome.Api.Controllers;

[Route("api/v1/health-reports")]
[ApiController]
public class HealthReportsController : ControllerBase
{
    private readonly ISender _sender;

    public HealthReportsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Retrieves a health report by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the report.</param>
    /// <returns>The health report details.</returns>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(HealthReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetReportByIdQuery(id);
        Result<HealthReportDto> result = await _sender.Send(query);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }
}
