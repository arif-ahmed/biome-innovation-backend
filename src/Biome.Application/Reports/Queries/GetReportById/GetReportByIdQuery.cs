using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Reports.Queries.GetReportById;

public sealed record GetReportByIdQuery(Guid ReportId) : IRequest<Result<HealthReportDto>>;
