using Biome.Domain.Shipping;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Shipping.Commands.CreateShipment;

public sealed class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Result<Guid>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IShippingService _shippingService;

    public CreateShipmentCommandHandler(IShipmentRepository shipmentRepository, IShippingService shippingService)
    {
        _shipmentRepository = shipmentRepository;
        _shippingService = shippingService;
    }

    public async Task<Result<Guid>> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        // Check for existing shipment?
        var existing = await _shipmentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        if (existing is not null)
        {
            return Result.Failure<Guid>(new Error("Shipment.AlreadyExists", "Shipment already exists for this order."));
        }

        var shipment = Shipment.Create(request.OrderId, request.Carrier, request.DestinationAddress);

        // Generate Label (External Call + State Change)
        var labelResult = await shipment.GenerateLabel(_shippingService);
        if (labelResult.IsFailure)
        {
            return Result.Failure<Guid>(labelResult.Error);
        }

        await _shipmentRepository.AddAsync(shipment, cancellationToken);
        await _shipmentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return shipment.Id;
    }
}
