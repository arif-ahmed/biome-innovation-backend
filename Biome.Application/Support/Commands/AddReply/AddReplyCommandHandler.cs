using Biome.Domain.Support;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Support.Commands.AddReply;

public sealed class AddReplyCommandHandler : IRequestHandler<AddReplyCommand, Result>
{
    private readonly ITicketRepository _ticketRepository;

    public AddReplyCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Result> Handle(AddReplyCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);

        if (ticket is null)
        {
            return Result.Failure(new Error("Ticket.NotFound", "Ticket not found."));
        }

        // Logic to verify ticket ownership could go here, but omitted for MVP unless required by strict rules

        ticket.AddReply(request.UserId, request.Content, request.IsFromCustomer);

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);
        await _ticketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
