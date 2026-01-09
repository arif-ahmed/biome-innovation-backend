using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Enums;


namespace Biome.SharedKernel.Abstractions;

public interface IShippingService
{
    Task<string> GenerateTrackingNumberAsync(Carrier carrier, Guid orderId);
    Task<byte[]> GenerateLabelAsync(Carrier carrier, string trackingNumber);
}
