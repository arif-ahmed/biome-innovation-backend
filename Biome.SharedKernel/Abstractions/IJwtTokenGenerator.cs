namespace Biome.SharedKernel.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string firstName, string lastName, string email, string role);
}
