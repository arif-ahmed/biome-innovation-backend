namespace Biome.Domain.Users.ValueObjects;

using Biome.SharedKernel.Core;

public sealed class UserRole : ValueObject
{
    public static readonly UserRole Customer = new(1, "Customer");
    public static readonly UserRole LabAssistant = new(2, "LabAssistant");
    public static readonly UserRole Admin = new(3, "Admin");

    private UserRole(int code, string name)
    {
        Code = code;
        Name = name;
    }

    public int Code { get; }
    public string Name { get; }

    public static UserRole? FromCode(int code)
    {
        return code switch
        {
            1 => Customer,
            2 => LabAssistant,
            3 => Admin,
            _ => null
        };
    }

    public static IEnumerable<UserRole> GetAll()
    {
        yield return Customer;
        yield return LabAssistant;
        yield return Admin;
    }

    public static bool TryFromName(string name, bool ignoreCase, out UserRole? role)
    {
        var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        role = GetAll().FirstOrDefault(r => string.Equals(r.Name, name, comparison));
        return role != null;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Code;
        yield return Name;
    }
}
