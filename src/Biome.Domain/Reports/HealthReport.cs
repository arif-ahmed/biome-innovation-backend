using Biome.SharedKernel.Core;
using Biome.Domain.Reports.Events;

namespace Biome.Domain.Reports;

public sealed class HealthReport : AggregateRoot
{
    // Constructor
    private HealthReport(Guid id, Guid labTestId, Guid petId, string reportContent, int healthScore) : base(id)
    {
        LabTestId = labTestId;
        PetId = petId;
        ReportContent = reportContent;
        HealthScore = healthScore;
        GeneratedAt = DateTime.UtcNow;
    }

    public Guid LabTestId { get; private set; }
    public Guid PetId { get; private set; }
    public string ReportContent { get; private set; }
    public int HealthScore { get; private set; }
    public DateTime GeneratedAt { get; private set; }

    // Factory method
    public static HealthReport Generate(Guid labTestId, Guid petId, string content, int score)
    {
        var report = new HealthReport(Guid.NewGuid(), labTestId, petId, content, score);
        report.RaiseDomainEvent(new ReportGeneratedDomainEvent(report.Id, petId, score));
        return report;
    }
}
