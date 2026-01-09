using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Lab.Commands.RecordTestResults;

public sealed record RecordLabTestResultsCommand(Guid OrderId, string RawDataJson) : IRequest<Result>;
