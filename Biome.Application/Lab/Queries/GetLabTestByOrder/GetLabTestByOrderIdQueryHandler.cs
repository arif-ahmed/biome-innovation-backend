using Biome.Domain.Lab;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Lab.Queries.GetLabTestByOrder;

public sealed class GetLabTestByOrderIdQueryHandler : IRequestHandler<GetLabTestByOrderIdQuery, Result<LabTestDto>>
{
    private readonly ILabTestRepository _labTestRepository;

    public GetLabTestByOrderIdQueryHandler(ILabTestRepository labTestRepository)
    {
        _labTestRepository = labTestRepository;
    }

    public async Task<Result<LabTestDto>> Handle(GetLabTestByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var labTest = await _labTestRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        if (labTest is null)
        {
            return Result.Failure<LabTestDto>(new Error("LabTest.NotFound", "Lab Test not found."));
        }

        return new LabTestDto(
            labTest.Id,
            labTest.OrderId,
            labTest.PetId,
            labTest.Status,
            labTest.RegisteredAt,
            labTest.ReceivedAt,
            labTest.CompletedAt,
            labTest.RawDataJson
        );
    }
}
