using Biome.Domain.Support.Entities;
using Biome.Domain.Support.Enums;
using Biome.SharedKernel.Core;
using Biome.SharedKernel.Primitives;

namespace Biome.Domain.Support;

public sealed class Ticket : AggregateRoot
{
    private readonly List<TicketMessage> _messages = new();

    private Ticket(Guid id, Guid customerId, string subject) : base(id)
    {
        CustomerId = customerId;
        Subject = subject;
        Status = SupportTicketStatus.Open;
        CreatedAt = DateTime.UtcNow;
        LastActivityAt = DateTime.UtcNow;
    }

    public Guid CustomerId { get; private set; }
    public string Subject { get; private set; }
    public SupportTicketStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastActivityAt { get; private set; }
    public IReadOnlyCollection<TicketMessage> Messages => _messages.AsReadOnly();

    public static Result<Ticket> Create(Guid customerId, string subject, string initialMessage)
    {
        if (string.IsNullOrWhiteSpace(subject))
            return Result.Failure<Ticket>(new Error("Ticket.EmptySubject", "Subject is required."));

        if (string.IsNullOrWhiteSpace(initialMessage))
            return Result.Failure<Ticket>(new Error("Ticket.EmptyMessage", "Initial message is required."));

        var ticket = new Ticket(Guid.NewGuid(), customerId, subject);
        
        // Initial message from the customer
        ticket.AddReply(customerId, initialMessage, isFromCustomer: true);
        
        return ticket;
    }

    public void AddReply(Guid authorId, string content, bool isFromCustomer)
    {
        var message = new TicketMessage(Guid.NewGuid(), authorId, content, isFromCustomer);
        _messages.Add(message);
        
        LastActivityAt = DateTime.UtcNow;
        
        // If customer replies, reopen ticket if defined logic requires it, or just keep status
        // If admin replies, change to InProgress? 
        if (!isFromCustomer && Status == SupportTicketStatus.Open)
        {
            Status = SupportTicketStatus.InProgress;
        }
    }

    public void Resolve()
    {
        Status = SupportTicketStatus.Resolved;
        LastActivityAt = DateTime.UtcNow;
    }
}
