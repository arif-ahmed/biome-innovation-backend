namespace Biome.Domain.Users.ValueObjects;

using Biome.SharedKernel.Core;
using Biome.SharedKernel.Primitives;

public sealed class FirstName : ValueObject
{
    public const int MaxLength = 100;

    private FirstName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<FirstName> Create(string? firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure<FirstName>(Error.NullValue);
        }

        if (firstName.Length > MaxLength)
        {
            return Result.Failure<FirstName>(new Error("FirstName.TooLong", "First name is too long"));
        }

        return new FirstName(firstName);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
