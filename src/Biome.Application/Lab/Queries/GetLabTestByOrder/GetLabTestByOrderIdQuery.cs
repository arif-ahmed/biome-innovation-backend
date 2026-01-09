using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Lab.Queries.GetLabTestByOrder;

public sealed record GetLabTestByOrderIdQuery(Guid OrderId) : IRequest<Result<LabTestDto>>;
