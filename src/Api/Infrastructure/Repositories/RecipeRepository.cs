using Microsoft.EntityFrameworkCore;
using Recipes.Api.Domain.Entities.Recipes;
using Recipes.Api.Domain.Interfaces.Recipes;
using Recipes.Api.Infrastructure.Data.Context;
using Recipes.Api.Infrastructure.Data.Extensions;

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

    public async Task<Recipe?> GetByIdForUser(int id, int userId)
    {
        return await _dbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(recipe => recipe.Category)
            .Include(recipe => recipe.Ingredients)
                .ThenInclude(ingredient => ingredient.Unit)
            .Include(recipe => recipe.Steps)
            .FirstOrDefaultAsync(recipe => recipe.Id == id && recipe.UserId == userId);
    }

    public async Task<Recipe?> GetById(int id)
    {
        return await _dbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(recipe => recipe.Category)
            .Include(recipe => recipe.Ingredients)
                .ThenInclude(ingredient => ingredient.Unit)
            .Include(recipe => recipe.Steps)
            .FirstOrDefaultAsync(recipe => recipe.Id == id);
    }

    public async Task<Recipe?> Create(Recipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        await _dbSet.AddAsync(recipe);
        await context.SaveChangesAsync();
        return await SelectDetachedAsync(recipe.Id);
    }

    public async Task<Recipe?> Update(Recipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        if (context.Entry(recipe).State == EntityState.Modified)
        {
            await context.SaveChangesAsync();
        }
        else
        {
            var existing = await SelectTrackedAsync(recipe.Id);
            if (existing is null)
                return null;

            context.TrackChildChanges(
                recipe.Ingredients,
                existing.Ingredients,
                (incoming, current) => incoming.Id > 0 && incoming.Id == current.Id);
            context.TrackChildChanges(
                recipe.Steps,
                existing.Steps,
                (incoming, current) => incoming.Id > 0 && incoming.Id == current.Id);

            await context.SaveDetachedChangesAsync(recipe, existing);
        }

        return await SelectDetachedAsync(recipe.Id);
    }

    public async Task<bool> CanAccessRecipe(int recipeId, int userId)
    {
        return await _dbSet.AnyAsync(recipe => recipe.Id == recipeId && recipe.UserId == userId);
    }

    private async Task<Recipe?> SelectTrackedAsync(int id)
    {
        return await _dbSet
            .AsSplitQuery()
            .Include(recipe => recipe.Category)
            .Include(recipe => recipe.Ingredients)
                .ThenInclude(ingredient => ingredient.Unit)
            .Include(recipe => recipe.Steps)
            .FirstOrDefaultAsync(recipe => recipe.Id == id);
    }

    private async Task<Recipe?> SelectDetachedAsync(int id)
    {
        return await _dbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(recipe => recipe.Category)
            .Include(recipe => recipe.Ingredients)
                .ThenInclude(ingredient => ingredient.Unit)
            .Include(recipe => recipe.Steps)
            .FirstOrDefaultAsync(recipe => recipe.Id == id);
    }
}
