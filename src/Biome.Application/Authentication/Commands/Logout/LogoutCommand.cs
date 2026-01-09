using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Authentication.Commands.Logout;

public sealed record LogoutCommand(Guid UserId) : IRequest<Result>;
