using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Infrastructure.Data.Configurations;

public class StepConfiguration : IEntityTypeConfiguration<Step>
{
    public void Configure(EntityTypeBuilder<Step> builder)
    {
        builder.ToTable("Steps");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Position)
            .IsRequired();
        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(2000);
        builder.Property(e => e.RecipeId)
            .IsRequired();

        builder.HasOne(e => e.Recipe)
            .WithMany()
            .HasForeignKey(e => e.RecipeId);

        builder.HasIndex(e => new { e.RecipeId, e.Position });
    }
}
