using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Authentication.Queries.GenerateTwoFactorSecret;

public sealed class GenerateTwoFactorSecretQueryHandler : IRequestHandler<GenerateTwoFactorSecretQuery, Result<string>>
{
    private readonly ITwoFactorService _twoFactorService;

    public GenerateTwoFactorSecretQueryHandler(ITwoFactorService twoFactorService)
    {
        _twoFactorService = twoFactorService;
    }

    public Task<Result<string>> Handle(GenerateTwoFactorSecretQuery request, CancellationToken cancellationToken)
    {
        // In real world, we might want to check if User exists, but this query is just a utility 
        // delegated to the service usually.
        // However, standard is to use the Service.
        var secret = _twoFactorService.GenerateSecret();
        return Task.FromResult(Result.Success(secret));
    }
}
