using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Support.Queries.GetTickets;

public sealed record GetCustomerTicketsQuery(Guid CustomerId) : IRequest<Result<List<TicketDto>>>;
