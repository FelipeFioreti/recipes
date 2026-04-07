using Microsoft.EntityFrameworkCore;
using Recipes.Api.Domain.Entities.BaseEntities;
using Recipes.Api.Domain.Entities.Recipes;
using Recipes.Api.Domain.Entities.Users;

namespace Recipes.Api.Infrastructure.Data.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeType> RecipeTypes { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    private void ApplyAuditFields()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreatedAt();
                    entry.Entity.SetUpdatedAt();
                    continue;
                case EntityState.Modified:
                    entry.Entity.SetUpdatedAt();
                    continue;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                default:
                    continue;
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }
}