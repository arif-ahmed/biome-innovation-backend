namespace Biome.SharedKernel.ValueObjects;

using Biome.SharedKernel.Core;

public sealed class Money : ValueObject
{
    public static readonly Money Zero = new(0, "USD");
    public static readonly Money ZeroEur = new(0, "EUR");

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }
    public string Currency { get; }

    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies have to match");
        }

        return new Money(first.Amount + second.Amount, first.Currency);
    }

    public static Money operator -(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies have to match");
        }

        return new Money(first.Amount - second.Amount, first.Currency);
    }

    public static bool operator >(Money first, Money second) => first.Amount > second.Amount;

    public static bool operator <(Money first, Money second) => first.Amount < second.Amount;

    public static bool operator >=(Money first, Money second) => first.Amount >= second.Amount;

    public static bool operator <=(Money first, Money second) => first.Amount <= second.Amount;


    public static bool operator ==(Money first, Money second)
    {
        if (first is null && second is null)
        {
            return true;
        }

        if (first is null || second is null)
        {
            return false;
        }

        return first.Equals(second);
    }

    public static bool operator !=(Money first, Money second) => !(first == second);

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }
}
