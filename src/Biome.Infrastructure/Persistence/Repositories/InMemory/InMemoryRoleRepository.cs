namespace Biome.Infrastructure.Persistence.Repositories;

using Biome.Domain.Roles;
using Biome.SharedKernel.Abstractions;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public sealed class InMemoryRoleRepository : IRoleRepository
{
    private static readonly ConcurrentDictionary<Guid, Role> _roles = new();

    static InMemoryRoleRepository()
    {
        var customerRole = Role.Create("Customer", "Default customer role");
        customerRole.AddPermission(Permission.Create("Users:ReadSelf", "Read own profile"));
        _roles.TryAdd(customerRole.Id, customerRole);

        var adminRole = Role.Create("Admin", "Administrator role");
        adminRole.AddPermission(Permission.Create("Users:Create", "Create users"));
        adminRole.AddPermission(Permission.Create("Users:Read", "Read all users"));
        adminRole.AddPermission(Permission.Create("Roles:Create", "Create new roles"));
        _roles.TryAdd(adminRole.Id, adminRole);

        var labAssistantRole = Role.Create("LabAssistant", "Lab Assistant role");
        _roles.TryAdd(labAssistantRole.Id, labAssistantRole);
    }

    IUnitOfWork IRoleRepository.UnitOfWork => null!;

    void IRoleRepository.Add(Role role)
    {
        _roles.TryAdd(role.Id, role);
    }

    Task<Role?> IRoleRepository.GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _roles.TryGetValue(id, out var role);
        return Task.FromResult<Role?>(role);
    }

    Task<Role?> IRoleRepository.GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var role = _roles.Values.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult<Role?>(role);
    }
}
