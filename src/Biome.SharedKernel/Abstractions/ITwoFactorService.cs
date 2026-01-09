using Biome.SharedKernel.Core;

namespace Biome.SharedKernel.Abstractions;

public interface ITwoFactorService
{
    string GenerateSecret();
    bool ValidateCode(string secret, string code);
}
