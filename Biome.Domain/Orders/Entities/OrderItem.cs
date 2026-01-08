using Biome.Domain.Orders.Enums;
using Biome.SharedKernel.Core;
using Biome.SharedKernel.Primitives;

namespace Biome.Domain.Orders.Entities;

public sealed class OrderItem : Entity
{
    internal OrderItem(Guid id, Guid productId, string productName, decimal unitPrice, int quantity, KitType kitType, Guid? petId)
        : base(id)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        KitType = kitType;
        PetId = petId;
    }

    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public KitType KitType { get; private set; }
    public Guid? PetId { get; private set; } // Optional link to a specific pet

    public decimal TotalAmount => UnitPrice * Quantity;
}
