using Biome.SharedKernel.Core;

namespace Biome.Domain.Support.Entities;

public sealed class TicketMessage : Entity
{
    internal TicketMessage(Guid id, Guid authorId, string content, bool isFromCustomer) 
        : base(id)
    {
        AuthorId = authorId;
        Content = content;
        IsFromCustomer = isFromCustomer;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid AuthorId { get; private set; }
    public string Content { get; private set; }
    public bool IsFromCustomer { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
