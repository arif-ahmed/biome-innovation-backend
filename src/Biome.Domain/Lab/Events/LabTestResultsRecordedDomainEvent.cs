using Biome.SharedKernel.Abstractions;

namespace Biome.Domain.Lab.Events;

public sealed record LabTestResultsRecordedDomainEvent(Guid LabTestId, Guid OrderId, Guid PetId) : IDomainEvent;
