using Biome.SharedKernel.Abstractions;

namespace Biome.Domain.Reports.Events;

public sealed record ReportGeneratedDomainEvent(Guid ReportId, Guid PetId, int HealthScore) : IDomainEvent;
