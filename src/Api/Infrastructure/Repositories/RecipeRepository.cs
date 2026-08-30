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
            .AsSplitQuery()
            .Include(recipe => recipe.Category)
            .Include(recipe => recipe.Ingredients)
            .ThenInclude(ingredient => ingredient.Unit)
            .Include(recipe => recipe.Steps)
            .OrderBy(x => x.Id)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetAllForUser(int userId, int page = 1, int size = 10)
    {
        return await _dbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(recipe => recipe.Category)
            .Include(recipe => recipe.Ingredients)
            .ThenInclude(ingredient => ingredient.Unit)
            .Include(recipe => recipe.Steps)
            .Where(recipe => recipe.UserId == userId)
            .OrderBy(x => x.Id)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<Recipe?> GetByIdForUser(int id, int userId, bool tracked = false)
    {
        return await SelectRecipe(tracked)
            .FirstOrDefaultAsync(recipe => recipe.Id == id && recipe.UserId == userId);
    }

    public async Task<Recipe?> GetById(int id, bool tracked = false)
    {
        return await SelectRecipe(tracked)
            .FirstOrDefaultAsync(recipe => recipe.Id == id);
    }

    public async Task<Recipe?> Create(Recipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        await _dbSet.AddAsync(recipe);
        await context.SaveChangesAsync();
        return await GetById(recipe.Id);
    }

    public async Task<Recipe?> Update(Recipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        await context.SaveChangesAsync();
        return await GetById(recipe.Id);
    }

    public async Task<bool> CanAccessRecipe(int recipeId, int userId)
    {
        return await _dbSet.AnyAsync(recipe => recipe.Id == recipeId && recipe.UserId == userId);
    }

    private IQueryable<Recipe> SelectRecipe(bool tracked)
    {
        var query = _dbSet
            .AsSplitQuery()
            .Include(recipe => recipe.Category)
            .Include(recipe => recipe.Ingredients)
            .ThenInclude(ingredient => ingredient.Unit)
            .Include(recipe => recipe.Steps
                .OrderBy(s => s.Position));

        return tracked ? query : query.AsNoTracking();
    }
}