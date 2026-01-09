using Biome.Domain.Payments;
using Biome.Domain.Payments.Events;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Primitives;

namespace Biome.Domain.Payments;

public interface IPaymentRepository
{
    void Add(Payment payment);
    IUnitOfWork UnitOfWork { get; }
}
