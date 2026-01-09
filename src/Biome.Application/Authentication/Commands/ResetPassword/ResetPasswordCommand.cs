using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Authentication.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result>;
