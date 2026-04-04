using Microsoft.EntityFrameworkCore;
using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Recipes;
using Recipes.Infrastructure.Data.Context;

namespace Recipes.Infrastructure.Repositories;

public class RecipeRepository(ApplicationDbContext context) : IRecipeRepository
{
    private readonly DbSet<Recipe> _dbSet = context.Recipes;

    public async Task<IEnumerable<Recipe>> GetAll()
    {
        return await _dbSet.ToListAsync();
    }


    public async Task<Recipe?> GetById(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<Recipe?> Create(Recipe recipe)
    {
        await _dbSet.AddAsync(recipe);
        await context.SaveChangesAsync();
        return recipe;
    }

    public async Task<Recipe?> Update(Recipe recipe)
    {
        _dbSet.Update(recipe);
        await context.SaveChangesAsync();
        return recipe;
    }

    public async Task Delete(Recipe recipe)
    {
        recipe.DeletedAt = DateTime.UtcNow;
        recipe.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(recipe);
        await context.SaveChangesAsync();
    }
}