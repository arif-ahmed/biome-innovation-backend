namespace Biome.Domain.Roles;

using Biome.SharedKernel.Core;

public sealed class Permission : ValueObject
{
    public string Code { get; private set; }
    public string Description { get; private set; }

    private Permission(string code, string description)
    {
        Code = code;
        Description = description;
    }

    public static Permission Create(string code, string description)
    {
        return new Permission(code, description);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Code;
        yield return Description;
    }
}
