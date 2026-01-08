namespace Biome.Domain.Users.ValueObjects;

using Biome.SharedKernel.Core;
using Biome.SharedKernel.Primitives;

public sealed class LastName : ValueObject
{
    public const int MaxLength = 100;

    private LastName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<LastName> Create(string? lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result.Failure<LastName>(Error.NullValue);
        }

        if (lastName.Length > MaxLength)
        {
            return Result.Failure<LastName>(new Error("LastName.TooLong", "Last name is too long"));
        }

        return new LastName(lastName);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
