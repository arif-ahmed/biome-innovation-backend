using Biome.Domain.Lab;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Lab.Commands.RecordTestResults;

public sealed class RecordLabTestResultsCommandHandler : IRequestHandler<RecordLabTestResultsCommand, Result>
{
    private readonly ILabTestRepository _labTestRepository;

    public RecordLabTestResultsCommandHandler(ILabTestRepository labTestRepository)
    {
        _labTestRepository = labTestRepository;
    }

    public async Task<Result> Handle(RecordLabTestResultsCommand request, CancellationToken cancellationToken)
    {
        var labTest = await _labTestRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        
        if (labTest is null)
        {
             return Result.Failure(new Error("LabTest.NotFound", $"No Lab Test found for Order {request.OrderId}"));
        }

        labTest.RecordResults(request.RawDataJson);

        await _labTestRepository.UpdateAsync(labTest, cancellationToken);
        await _labTestRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
