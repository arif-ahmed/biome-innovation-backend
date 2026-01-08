using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Roles.Commands.AssignPermissions;

public sealed record AssignPermissionsToRoleCommand(Guid RoleId, List<string> Permissions) : IRequest<Result>;
