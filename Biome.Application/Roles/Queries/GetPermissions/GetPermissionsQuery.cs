using Biome.Application.Roles.Common;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Roles.Queries.GetPermissions;

public sealed record GetPermissionsQuery() : IRequest<Result<List<PermissionResponse>>>;
