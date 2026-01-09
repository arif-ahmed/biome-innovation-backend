using Biome.Application.Authentication.Common;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Authentication.Commands.LoginTwoFactor;

public sealed record LoginTwoFactorCommand(string Email, string Code) : IRequest<Result<AuthenticationResult>>;
