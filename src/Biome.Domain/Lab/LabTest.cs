using Biome.Domain.Lab.Enums;
using Biome.Domain.Lab.Events;
using Biome.SharedKernel.Core;

namespace Biome.Domain.Lab;

public sealed class LabTest : AggregateRoot
{
    private LabTest(Guid id, Guid orderId, Guid petId) : base(id)
    {
        OrderId = orderId;
        PetId = petId;
        Status = LabTestStatus.Registered;
        RegisteredAt = DateTime.UtcNow;
    }

    public Guid OrderId { get; private set; }
    public Guid PetId { get; private set; }
    public LabTestStatus Status { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? RawDataJson { get; private set; }

    public static LabTest Create(Guid orderId, Guid petId)
    {
        // Enforce invariants here if needed
        return new LabTest(Guid.NewGuid(), orderId, petId);
    }

    public void ReceiveSample()
    {
        // Simple state transition for MVP
        Status = LabTestStatus.SampleReceived;
        ReceivedAt = DateTime.UtcNow;
    }

    public void StartProcessing()
    {
        if (Status == LabTestStatus.SampleReceived)
        {
            Status = LabTestStatus.Processing;
        }
    }

    public void RecordResults(string rawDataJson)
    {
        // Can only record results if processing or received
        if (Status == LabTestStatus.Failed) return;

        Status = LabTestStatus.AnalysisCompleted;
        RawDataJson = rawDataJson;
        CompletedAt = DateTime.UtcNow;
        
        RaiseDomainEvent(new LabTestResultsRecordedDomainEvent(Id, OrderId, PetId));
    }

    public void FailTest(string reason)
    {
        Status = LabTestStatus.Failed;
        // Logic to track failure reason could be added to properties
    }
}
