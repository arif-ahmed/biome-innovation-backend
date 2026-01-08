using System;
using System.Collections.Generic;
using Biome.SharedKernel.Core;
using Biome.SharedKernel.Primitives;

namespace Biome.SharedKernel.ValueObjects;

public sealed class Money : ValueObject
{
    public static readonly Money Zero = new(0, "USD");

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }
    public string Currency { get; }

    public static Money From(decimal amount, string currency)
    {
        return new Money(amount, currency);
    }

    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies must match for addition");
        }

        return new Money(first.Amount + second.Amount, first.Currency);
    }

    public static Money operator -(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies must match for subtraction");
        }

        return new Money(first.Amount - second.Amount, first.Currency);
    }

    public static Money operator *(Money first, int multiplier)
    {
        return new Money(first.Amount * multiplier, first.Currency);
    }

    public static bool operator ==(Money? left, Money? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Money? left, Money? right)
    {
        return !Equals(left, right);
    }

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
