using Biome.SharedKernel.Abstractions;

namespace Biome.Infrastructure.Services.Authentication;

public sealed class MockTwoFactorService : ITwoFactorService
{
    public string GenerateSecret()
    {
        return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();
    }

    public bool ValidateCode(string secret, string code)
    {
        // For MVP/Mock, any code "123456" is valid, or maybe code == "123456" for simplicity.
        // In real world, use OtpNet library.
        return code == "123456";
    }
}
