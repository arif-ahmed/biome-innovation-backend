using Biome.Domain.Users;
using Biome.Domain.Users.ValueObjects;
using Biome.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biome.Infrastructure.Persistence.Configurations.Postgres;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasConversion(x => x.Value, x => FirstName.Create(x).Value)
            .HasMaxLength(FirstName.MaxLength)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasConversion(x => x.Value, x => LastName.Create(x).Value)
            .HasMaxLength(LastName.MaxLength)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasConversion(x => x.Value, x => Email.Create(x).Value)
            .IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.PasswordHash).IsRequired();

        builder.OwnsOne(x => x.RefreshToken, token =>
        {
            token.Property(x => x.Token).HasColumnName("RefreshToken");
            token.Property(x => x.ExpiryOnUtc).HasColumnName("RefreshTokenExpiry");
        });

        builder.OwnsOne(x => x.PasswordReset, reset =>
        {
            reset.Property(x => x.Token).HasColumnName("PasswordResetToken");
            reset.Property(x => x.Expiry).HasColumnName("PasswordResetExpiry");
        });

        // Ignore methods/properties that are not stored

    }
}
