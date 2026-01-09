using Biome.Domain.Reports;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Reports.Queries.GetReportById;

public sealed class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, Result<HealthReportDto>>
{
    private readonly IHealthReportRepository _repository;

    public GetReportByIdQueryHandler(IHealthReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<HealthReportDto>> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        var report = await _repository.GetByIdAsync(request.ReportId, cancellationToken);
        if (report is null)
        {
            return Result.Failure<HealthReportDto>(new Error("Report.NotFound", "Health Report not found."));
        }

        return new HealthReportDto(
            report.Id,
            report.PetId,
            report.HealthScore,
            report.ReportContent,
            report.GeneratedAt
        );
    }
}
