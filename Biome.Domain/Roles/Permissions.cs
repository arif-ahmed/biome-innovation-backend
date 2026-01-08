namespace Biome.Domain.Roles;

public static class Permissions
{
    public static readonly Permission UsersRead = Permission.Create("Users:Read", "Read user information");
    public static readonly Permission UsersCreate = Permission.Create("Users:Create", "Create new users");
    public static readonly Permission UsersUpdate = Permission.Create("Users:Update", "Update user information");
    public static readonly Permission UsersDelete = Permission.Create("Users:Delete", "Delete users");
    public static readonly Permission RolesRead = Permission.Create("Roles:Read", "Read role information");
    public static readonly Permission RolesCreate = Permission.Create("Roles:Create", "Create new roles");
    public static readonly Permission RolesUpdate = Permission.Create("Roles:Update", "Update role information");
    public static readonly Permission RolesDelete = Permission.Create("Roles:Delete", "Delete roles");
    
    public static IReadOnlyCollection<Permission> All = new[]
    {
        UsersRead, UsersCreate, UsersUpdate, UsersDelete,
        RolesRead, RolesCreate, RolesUpdate, RolesDelete
    };

    public static Permission? GetByCode(string code)
    {
        return All.FirstOrDefault(p => p.Code == code);
    }
}
