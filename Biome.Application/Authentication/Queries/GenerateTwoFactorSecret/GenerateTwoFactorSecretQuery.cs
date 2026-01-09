using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Authentication.Queries.GenerateTwoFactorSecret;

public sealed record GenerateTwoFactorSecretQuery(Guid UserId) : IRequest<Result<string>>;
