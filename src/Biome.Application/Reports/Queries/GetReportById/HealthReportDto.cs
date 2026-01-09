namespace Biome.Application.Reports.Queries.GetReportById;

public record HealthReportDto(
    Guid Id, 
    Guid PetId, 
    int HealthScore, 
    string ReportContent, 
    DateTime GeneratedAt
);
