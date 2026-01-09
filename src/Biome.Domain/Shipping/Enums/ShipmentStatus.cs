namespace Biome.Domain.Shipping.Enums;

public enum ShipmentStatus
{
    Created = 1,
    LabelGenerated = 2,
    Shipped = 3,
    InTransit = 4,
    Delivered = 5,
    Failed = 6
}
