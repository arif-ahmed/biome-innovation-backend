using Biome.Domain.Lab;
using Biome.Domain.Lab.Events;
using Biome.Domain.Reports;
using MediatR;

namespace Biome.Application.Reports.EventHandlers;

public sealed class LabTestResultsRecordedEventHandler : INotificationHandler<LabTestResultsRecordedDomainEvent>
{
    private readonly ILabTestRepository _labTestRepository;
    private readonly IHealthReportRepository _healthReportRepository;

    public LabTestResultsRecordedEventHandler(
        ILabTestRepository labTestRepository, 
        IHealthReportRepository healthReportRepository)
    {
        _labTestRepository = labTestRepository;
        _healthReportRepository = healthReportRepository;
    }

    public async Task Handle(LabTestResultsRecordedDomainEvent notification, CancellationToken cancellationToken)
    {
        var labTest = await _labTestRepository.GetByIdAsync(notification.LabTestId, cancellationToken);
        if (labTest is null)
        {
            return;
        }

        // Mock Analysis Logic
        int healthScore = new Random().Next(60, 100); 
        string reportContent = $"Analysis for Pet {notification.PetId}. Based on raw data: {labTest.RawDataJson}... Gut microbiome is balanced.";

        var report = HealthReport.Generate(labTest.Id, notification.PetId, reportContent, healthScore);

        await _healthReportRepository.AddAsync(report, cancellationToken);
        await _healthReportRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
