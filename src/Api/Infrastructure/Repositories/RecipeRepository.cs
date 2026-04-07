using Microsoft.EntityFrameworkCore;
using Recipes.Api.Domain.Entities.Recipes;
using Recipes.Api.Domain.Interfaces.Recipes;
using Recipes.Api.Infrastructure.Data.Context;

namespace Recipes.Api.Infrastructure.Repositories;

public class RecipeRepository(ApplicationDbContext context) : IRecipeRepository
{
    private readonly DbSet<Recipe> _dbSet = context.Recipes;

    public async Task<IEnumerable<Recipe>> GetAll(int userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(recipe => recipe.UserId == userId)
            .ToListAsync();
    }


    public async Task<Recipe?> GetById(int id, int userId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(recipe => recipe.Id == id && recipe.UserId == userId);
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
}
