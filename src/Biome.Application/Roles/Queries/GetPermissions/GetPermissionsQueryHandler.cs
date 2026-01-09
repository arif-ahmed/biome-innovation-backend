using Biome.Application.Roles.Common;
using Biome.Domain.Roles;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Roles.Queries.GetPermissions;

internal sealed class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, Result<List<PermissionResponse>>>
{
    public Task<Result<List<PermissionResponse>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = Permissions.All
            .Select(p => new PermissionResponse(p.Code, p.Description))
            .ToList();

        return Task.FromResult(Result.Success(permissions));
    }
}
