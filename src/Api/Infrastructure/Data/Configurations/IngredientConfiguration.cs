using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Infrastructure.Data.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("Ingredients");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(e => e.Quantity)
            .HasPrecision(18, 2)
            .IsRequired();
        builder.Property(e => e.RecipeId)
            .IsRequired();
        builder.Property(e => e.UnitId)
            .IsRequired();

        builder.HasOne(e => e.Recipe)
            .WithMany()
            .HasForeignKey(e => e.RecipeId);
        builder.HasOne(e => e.Unit)
            .WithMany()
            .HasForeignKey(e => e.UnitId);

        builder.HasIndex(e => e.Name);
    }
}
