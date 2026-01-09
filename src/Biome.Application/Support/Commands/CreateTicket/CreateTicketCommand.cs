using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Support.Commands.CreateTicket;

public sealed record CreateTicketCommand(Guid CustomerId, string Subject, string InitialMessage) : IRequest<Result<Guid>>;
