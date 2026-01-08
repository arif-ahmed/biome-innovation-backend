using Biome.Domain.Orders.Enums;

namespace Biome.Application.Orders.Commands.CreateOrder;

public sealed record OrderItemDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    KitType KitType,
    Guid? PetId);
