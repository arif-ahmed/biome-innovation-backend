namespace Biome.Application.Authentication.Common;

using Biome.Domain.Users;

public sealed record AuthenticationResult(
    User User,
    string Token);
