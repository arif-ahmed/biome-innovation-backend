using Biome.Domain.Payments.Enums;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Core;
using Biome.SharedKernel.Primitives;
using Biome.SharedKernel.ValueObjects;

namespace Biome.Domain.Payments.Events;

public sealed record PaymentSucceededDomainEvent(Guid PaymentId, Guid OrderId) : IDomainEvent;

public sealed class Payment : AggregateRoot
{
    private Payment(Guid id, Guid orderId, Money amount, PaymentStatus status)
        : base(id)
    {
        OrderId = orderId;
        Amount = amount;
        Status = status;
    }

    public Guid OrderId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public string? FailureReason { get; private set; }

    public static Payment Create(Guid orderId, Money amount)
    {
        return new Payment(Guid.NewGuid(), orderId, amount, PaymentStatus.Pending);
    }

    public void MarkAsCompleted(string gatewayTransactionId)
    {
        if (Status != PaymentStatus.Pending)
        {
            return;
        }

        Status = PaymentStatus.Completed;
        GatewayTransactionId = gatewayTransactionId;

        RaiseDomainEvent(new PaymentSucceededDomainEvent(Id, OrderId));
    }

    public void MarkAsFailed(string failureReason)
    {
        if (Status != PaymentStatus.Pending)
        {
            return;
        }

        Status = PaymentStatus.Failed;
        FailureReason = failureReason;
    }
}
