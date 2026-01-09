namespace Biome.Application.Authentication.Commands.Login;

using Biome.Application.Authentication.Common;
using Biome.SharedKernel.Primitives;
using MediatR;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthenticationResult>>;
