using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Api.Domain.Entities.Admin;

namespace Recipes.Api.Infrastructure.Data.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.Abbreviation)
            .HasMaxLength(10);

        builder.HasIndex(e => e.Name);
    }
}
