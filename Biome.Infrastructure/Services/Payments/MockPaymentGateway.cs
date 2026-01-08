using Biome.Application.Common.Interfaces;
using Biome.SharedKernel.ValueObjects;

namespace Biome.Infrastructure.Services.Payments;

public class MockPaymentGateway : IPaymentGateway
{
    public async Task<string> ChargeAsync(Money amount, string token, CancellationToken cancellationToken)
    {
        // Simulate delay
        await Task.Delay(1000, cancellationToken);

        // Simulate success. In a real app, we'd check the token.
        return Guid.NewGuid().ToString(); 
    }
}
