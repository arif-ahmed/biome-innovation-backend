using Biome.Domain.Lab.Enums;

namespace Biome.Application.Lab.Queries.GetLabTestByOrder;

public record LabTestDto(
    Guid Id, 
    Guid OrderId, 
    Guid PetId, 
    LabTestStatus Status, 
    DateTime RegisteredAt,
    DateTime? ReceivedAt,
    DateTime? CompletedAt,
    string? RawDataJson
);
