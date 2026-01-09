using Biome.SharedKernel.Enums;
using Biome.Domain.Shipping.ValueObjects;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Core;
using Biome.SharedKernel.Primitives;
using Biome.Domain.Shipping.Enums;

namespace Biome.Domain.Shipping;

public sealed class Shipment : AggregateRoot
{
    private Shipment(Guid id, Guid orderId, Carrier carrier, string destinationAddress) : base(id)
    {
        OrderId = orderId;
        Carrier = carrier;
        DestinationAddress = destinationAddress; // Simplify address for MVP
        Status = ShipmentStatus.Created;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid OrderId { get; private set; }
    public Carrier Carrier { get; private set; }
    public string DestinationAddress { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public TrackingNumber? TrackingNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }

    public static Shipment Create(Guid orderId, Carrier carrier, string destinationAddress)
    {
        return new Shipment(Guid.NewGuid(), orderId, carrier, destinationAddress);
    }

    public async Task<Result> GenerateLabel(IShippingService shippingService)
    {
        if (Status != ShipmentStatus.Created)
        {
            return Result.Failure(new Error("Shipment.InvalidState", "Label already generated or shipped."));
        }

        var trackingNumberStr = await shippingService.GenerateTrackingNumberAsync(Carrier, OrderId);
        TrackingNumber = TrackingNumber.Create(trackingNumberStr);
        
        Status = ShipmentStatus.LabelGenerated;
        
        // In real world, we would store the PDF bytes here or in S3.
        
        return Result.Success();
    }

    public void MarkAsShipped()
    {
        if (Status != ShipmentStatus.LabelGenerated) return;

        Status = ShipmentStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
    }
}
