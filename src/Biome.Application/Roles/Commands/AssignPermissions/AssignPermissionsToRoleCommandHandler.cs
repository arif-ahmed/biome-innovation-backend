using Biome.Domain.Roles;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Roles.Commands.AssignPermissions;

internal sealed class AssignPermissionsToRoleCommandHandler : IRequestHandler<AssignPermissionsToRoleCommand, Result>
{
    private readonly IRoleRepository _roleRepository;

    public AssignPermissionsToRoleCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result> Handle(AssignPermissionsToRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
        {
            return Result.Failure(new Error("Role.NotFound", "Role not found."));
        }

        // Logic to sync permissions
        // 1. Find permissions to remove
        var currentPermissionCodes = role.Permissions.Select(p => p.Code).ToList();
        var permissionsToRemove = currentPermissionCodes.Except(request.Permissions).ToList();
        
        foreach (var code in permissionsToRemove)
        {
            role.RemovePermission(code);
        }

        // 2. Find permissions to add
        var permissionsToAdd = request.Permissions.Except(currentPermissionCodes).ToList();
        
        foreach (var code in permissionsToAdd)
        {
            var permission = Permissions.GetByCode(code);
            if (permission != null)
            {
                role.AddPermission(permission);
            }
            // If permission code is unknown, ignore or error? Ignoring for now to be safe.
        }

        await _roleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
