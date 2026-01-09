using Biome.Application.Authentication.Common;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Authentication.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result>;
