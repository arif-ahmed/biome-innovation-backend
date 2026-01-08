namespace Biome.SharedKernel.ValueObjects;

using Biome.SharedKernel.Core;
using Biome.SharedKernel.Primitives;
using System.Text.RegularExpressions;

public sealed class Email : ValueObject
{
    public const int MaxLength = 255;
    private const string EmailRegexPattern = @"^[^@]+@[^@]+\.[^@]+$";

    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Email> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<Email>(Error.NullValue);
        }

        if (email.Length > MaxLength)
        {
            return Result.Failure<Email>(new Error("Email.TooLong", "Email is too long"));
        }

        if (!Regex.IsMatch(email, EmailRegexPattern))
        {
            return Result.Failure<Email>(new Error("Email.InvalidFormat", "Email format is invalid"));
        }

        return new Email(email);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
