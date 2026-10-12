using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain.Entities.Token;

namespace Recipes.Infrastructure.Data.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("PasswordResetTokens");

        builder.HasQueryFilter(e => e.User != null && e.User.DeletedAt == null);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.TokenHash)
            .IsRequired();

        builder.Property(e => e.ExpiresAt)
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.HasIndex(e => e.TokenHash)
            .IsUnique();

        builder.HasOne(r => r.User)
            .WithMany(u => u.PasswordResetTokens)
            .HasForeignKey(r => r.UserId);
    }
}
