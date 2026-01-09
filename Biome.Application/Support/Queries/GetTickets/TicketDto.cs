using Biome.Domain.Support.Enums;

namespace Biome.Application.Support.Queries.GetTickets;

public record TicketDto(
    Guid Id,
    Guid CustomerId,
    string Subject,
    SupportTicketStatus Status,
    DateTime CreatedAt,
    DateTime LastActivityAt,
    List<TicketMessageDto> Messages
);

public record TicketMessageDto(
    Guid Id,
    Guid AuthorId,
    string Content,
    bool IsFromCustomer,
    DateTime CreatedAt
);
