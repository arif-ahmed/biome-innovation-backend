using Biome.Domain.Support;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Support.Queries.GetTickets;

public sealed class GetCustomerTicketsQueryHandler : IRequestHandler<GetCustomerTicketsQuery, Result<List<TicketDto>>>
{
    private readonly ITicketRepository _ticketRepository;

    public GetCustomerTicketsQueryHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Result<List<TicketDto>>> Handle(GetCustomerTicketsQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

        var ticketDtos = tickets.Select(t => new TicketDto(
            t.Id,
            t.CustomerId,
            t.Subject,
            t.Status,
            t.CreatedAt,
            t.LastActivityAt,
            t.Messages.Select(m => new TicketMessageDto(
                m.Id,
                m.AuthorId,
                m.Content,
                m.IsFromCustomer,
                m.CreatedAt)).ToList()
        )).ToList();

        return ticketDtos;
    }
}
