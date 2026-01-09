namespace Biome.Domain.Roles;

using Biome.SharedKernel.Core;

public sealed class Role : AggregateRoot
{
    private readonly List<Permission> _permissions = new();

    private Role(Guid id, string name, string description) : base(id)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    public static Role Create(string name, string description)
    {
        return new Role(Guid.NewGuid(), name, description);
    }

    public void AddPermission(Permission permission)
    {
        if (!_permissions.Any(p => p.Code == permission.Code))
        {
            _permissions.Add(permission);
        }
    }

    public void RemovePermission(string permissionCode)
    {
        var permission = _permissions.FirstOrDefault(p => p.Code == permissionCode);
        if (permission != null)
        {
            _permissions.Remove(permission);
        }
    }
}
