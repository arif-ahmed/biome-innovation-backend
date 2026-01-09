using Biome.SharedKernel.Core;

namespace Biome.Domain.Shipping.ValueObjects;

public sealed class TrackingNumber : ValueObject
{
    public string Value { get; }

    private TrackingNumber(string value)
    {
        Value = value;
    }

    public static TrackingNumber Create(string value)
    {
        // Add regex logic for carrier formats if needed
        return new TrackingNumber(value);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
