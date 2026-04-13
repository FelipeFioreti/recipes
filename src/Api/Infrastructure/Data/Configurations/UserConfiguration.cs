using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Api.Domain.Entities.Enums;
using Recipes.Api.Domain.Entities.Users;

namespace Recipes.Api.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(e => e.Password)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(e => e.Role)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(Roles.USER);

        builder.HasIndex(u => u.Name);
        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}
