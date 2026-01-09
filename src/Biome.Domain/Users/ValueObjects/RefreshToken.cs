using Biome.SharedKernel.Core;
using Biome.SharedKernel.Primitives;

namespace Biome.Domain.Users.ValueObjects;

public sealed class RefreshToken : ValueObject
{
    public string Token { get; }
    public DateTime ExpiryOnUtc { get; }

    private RefreshToken(string token, DateTime expiryOnUtc)
    {
        Token = token;
        ExpiryOnUtc = expiryOnUtc;
    }
    
    public static RefreshToken Create(string token, DateTime expiryOnUtc)
    {
        return new RefreshToken(token, expiryOnUtc);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Token;
        yield return ExpiryOnUtc;
    }
}
