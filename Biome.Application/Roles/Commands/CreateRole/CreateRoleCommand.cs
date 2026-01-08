namespace Biome.Application.Roles.Commands.CreateRole;

using Biome.SharedKernel.Primitives;
using MediatR;

public sealed record CreateRoleCommand(string Name, string Description) : IRequest<Result<Guid>>;
