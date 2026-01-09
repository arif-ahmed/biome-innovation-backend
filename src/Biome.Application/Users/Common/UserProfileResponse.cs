namespace Biome.Application.Users.Common;

public sealed record UserProfileResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    bool IsEmailVerified);
