using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Authentication.Commands.EnableTwoFactor;

public sealed record EnableTwoFactorCommand(Guid UserId, string Secret, string Code) : IRequest<Result>;
