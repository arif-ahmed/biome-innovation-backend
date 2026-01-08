using Biome.Domain.Orders.Enums;

namespace Biome.Application.Orders.Common;

public sealed record OrderResponse(
    Guid Id,
    DateTime OrderDate,
    OrderStatus Status,
    decimal TotalAmount,
    List<OrderItemResponse> Items);

public sealed record OrderItemResponse(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    KitType KitType,
    Guid? PetId,
    decimal TotalAmount);
