using Biome.SharedKernel.Enums;
using Biome.SharedKernel.Abstractions;

namespace Biome.Infrastructure.Services.Shipping;

public sealed class MockShippingService : IShippingService
{
    public Task<byte[]> GenerateLabelAsync(Carrier carrier, string trackingNumber)
    {
        // Return dummy PDF bytes
        return Task.FromResult(new byte[10]);
    }

    public Task<string> GenerateTrackingNumberAsync(Carrier carrier, Guid orderId)
    {
        var tracking = $"TRK-{carrier}-{orderId.ToString().Substring(0, 8).ToUpper()}";
        return Task.FromResult(tracking);
    }
}
