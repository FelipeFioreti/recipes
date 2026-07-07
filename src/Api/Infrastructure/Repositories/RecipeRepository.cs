using Microsoft.EntityFrameworkCore;
using Recipes.Api.Domain.Entities.Recipes;
using Recipes.Api.Domain.Interfaces.Recipes;
using Recipes.Api.Infrastructure.Data.Context;

namespace Recipes.Api.Infrastructure.Repositories;

public class RecipeRepository(ApplicationDbContext context) : IRecipeRepository
{
    private readonly DbSet<Recipe> _dbSet = context.Recipes;

    public async Task<IEnumerable<Recipe>> GetAll(int page = 0, int size = 10)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetAllForUser(int userId, int page = 1, int size = 10)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(recipe => recipe.UserId == userId)
            .OrderBy(x => x.Id)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<Recipe?> GetByIdForUser(int id, int userId)
    {
        return await _dbSet
            .Include(recipe => recipe.Category)
            .FirstOrDefaultAsync(recipe => recipe.Id == id && recipe.UserId == userId);
    }

    public async Task<Recipe?> GetById(int id)
    {
        return await _dbSet
            .Include(recipe => recipe.Category)
            .FirstOrDefaultAsync(recipe => recipe.Id == id);
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
