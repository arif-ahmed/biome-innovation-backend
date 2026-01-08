using Biome.Domain.Orders.Entities;
using Biome.Domain.Orders.Enums;
using Biome.Domain.Orders.Events;
using Biome.SharedKernel.Core;
using Biome.SharedKernel.Primitives;

namespace Biome.Domain.Orders;

public sealed class Order : AggregateRoot
{
    private readonly List<OrderItem> _orderItems = new();

    private Order(Guid id, Guid customerId, DateTime orderDate, OrderStatus status)
        : base(id)
    {
        CustomerId = customerId;
        OrderDate = orderDate;
        Status = status;
    }

    public Guid CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public decimal TotalAmount => _orderItems.Sum(item => item.TotalAmount);

    public static Order Create(Guid customerId)
    {
        var order = new Order(Guid.NewGuid(), customerId, DateTime.UtcNow, OrderStatus.Pending);
        // We don't raise event here because an empty order is invalid. We raise it after adding items? 
        // Or we require items in Create? 
        // Decision: Create empty, then add items. But Invariant says "Must have items". 
        // So we will enforce invariant in "PlaceOrder" or require Items in Create.
        // Let's stick to simple "Create" factory and enforcing invariant at persistence or via "Confirm" method.
        // For MVP, we'll assume the Application Service adds items immediately.
        
        return order;
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity, KitType kitType, Guid? petId)
    {
        if (quantity <= 0)
        {
            // Normally throw exception or return Result
            return;
        }

        var orderItem = new OrderItem(Guid.NewGuid(), productId, productName, unitPrice, quantity, kitType, petId);
        _orderItems.Add(orderItem);
    }
    
    // Call this after adding items to finalize creation
    public Result FinalizeCreation()
    {
        if (!_orderItems.Any())
        {
            return Result.Failure(new Error("Order.NoItems", "Order must have at least one item."));
        }

        RaiseDomainEvent(new OrderCreatedDomainEvent(Id, CustomerId));
        return Result.Success();
    }

    public Result MarkAsPaid()
    {
        if (Status != OrderStatus.Pending)
        {
            return Result.Failure(new Error("Order.NotPending", "Only pending orders can be paid."));
        }

        Status = OrderStatus.Paid;
        RaiseDomainEvent(new OrderPaidDomainEvent(Id));
        return Result.Success();
    }
}
