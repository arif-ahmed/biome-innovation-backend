using Biome.Domain.Support;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Support.Commands.CreateTicket;

public sealed class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Result<Guid>>
{
    private readonly ITicketRepository _ticketRepository;

    public CreateTicketCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Result<Guid>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticketResult = Ticket.Create(request.CustomerId, request.Subject, request.InitialMessage);

        if (ticketResult.IsFailure)
        {
            return Result.Failure<Guid>(ticketResult.Error);
        }

        var ticket = ticketResult.Value;

        await _ticketRepository.AddAsync(ticket, cancellationToken);
        await _ticketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
