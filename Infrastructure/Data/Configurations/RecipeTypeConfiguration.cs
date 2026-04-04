using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain.Entities.Recipes;

namespace Recipes.Infrastructure.Data.Configurations;

public class RecipeTypeConfiguration : IEntityTypeConfiguration<RecipeType>
{
    public void Configure(EntityTypeBuilder<RecipeType> builder)
    {
        builder.ToTable("RecipeTypes");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(e => e.Name);
    }
}