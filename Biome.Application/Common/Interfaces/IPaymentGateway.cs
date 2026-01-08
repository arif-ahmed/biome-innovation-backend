using Biome.SharedKernel.ValueObjects;

namespace Biome.Application.Common.Interfaces;

public interface IPaymentGateway
{
    Task<string> ChargeAsync(Money amount, string token, CancellationToken cancellationToken);
}
