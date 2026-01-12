using Biome.Domain.Support;
using Biome.Domain.Support.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biome.Infrastructure.Persistence.Configurations.Postgres;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Subject).HasMaxLength(200).IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        // Map the backing field for Messages
        builder.Metadata.FindNavigation(nameof(Ticket.Messages))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(x => x.Messages, message =>
        {
            message.ToTable("TicketMessages");
            message.WithOwner().HasForeignKey("TicketId");
            message.HasKey(x => x.Id);
            message.Property(x => x.Content).IsRequired();
        });


    }
}
