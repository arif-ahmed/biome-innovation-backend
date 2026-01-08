using Biome.SharedKernel.Core;
using Biome.SharedKernel.Primitives;

namespace Biome.Domain.Users.ValueObjects;

public sealed class PasswordReset : ValueObject
{
    public PasswordReset(string token, DateTime expiry)
    {
        Token = token;
        Expiry = expiry;
    }

    public string Token { get; }
    public DateTime Expiry { get; }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Token;
        yield return Expiry;
    }
}
