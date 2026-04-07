using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Infrastructure.Data.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("Recipes");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(e => e.Description)
            .HasMaxLength(2000);
        builder.Property(e => e.UserId)
            .IsRequired();
        builder.Property(e => e.RecipeTypeId)
            .IsRequired();
        builder.HasOne(r => r.User)
            .WithMany(u => u.Recipes)
            .HasForeignKey(r => r.UserId);
        builder.HasOne(r => r.RecipeType)
            .WithMany(u => u.Recipes)
            .HasForeignKey(r => r.RecipeTypeId);

        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.Description);
    }
}