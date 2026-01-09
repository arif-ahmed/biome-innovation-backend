using Biome.Application.Common.Interfaces;
using Biome.Domain.Orders;
using Biome.Domain.Payments;
using Biome.Domain.Payments.Events;
using Biome.SharedKernel.Primitives;
using Biome.SharedKernel.ValueObjects;
using MediatR;

namespace Biome.Application.Payments.Commands.ProcessPayment;

public sealed record ProcessPaymentCommand(Guid OrderId, decimal Amount, string Currency, string Token) : IRequest<Result>;

internal sealed class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentGateway _paymentGateway;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IPaymentGateway paymentGateway)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _paymentGateway = paymentGateway;
    }

    public async Task<Result> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate Order exists (Optional, depending on strictness. 
        //    Payment BC might not need to know about Order Aggregate, just ID. 
        //    But checking existence is good practice.)
        //    However, to be strictly decoupled, we might just trust the ID or call an OrderService.
        //    For this monolith, we'll quickly check if order exists to prevent ghost payments.
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(new Error("Payment.OrderNotFound", "Order not found."));
        }
        
        // 2. Create Money
        var money = Money.From(request.Amount, request.Currency);

        // 3. Create Payment (Pending)
        var payment = Payment.Create(request.OrderId, money);
        _paymentRepository.Add(payment);

        try
        {
            // 4. Process with Gateway
            var transactionId = await _paymentGateway.ChargeAsync(money, request.Token, cancellationToken);

            // 5. Complete Payment (Raises Event)
            payment.MarkAsCompleted(transactionId);
        }
        catch (Exception ex)
        {
            payment.MarkAsFailed(ex.Message);
            // We still save the failed payment record
        }

        // 6. Save
        await _paymentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (payment.Status == Biome.Domain.Payments.Enums.PaymentStatus.Failed)
        {
            return Result.Failure(new Error("Payment.Failed", $"Payment failed: {payment.FailureReason}"));
        }

        return Result.Success();
    }
}
