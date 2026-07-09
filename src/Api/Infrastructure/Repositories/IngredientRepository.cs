using Microsoft.EntityFrameworkCore;
using Recipes.Api.Domain.Entities.Recipes;
using Recipes.Api.Domain.Interfaces.Recipes;
using Recipes.Api.Infrastructure.Data.Context;

namespace Recipes.Api.Infrastructure.Repositories;

public class IngredientRepository(ApplicationDbContext context) : IIngredientRepository
{
    private readonly DbSet<Ingredient> _dbSet = context.Ingredients;

    public async Task<IEnumerable<Ingredient>> GetAll(int page = 0, int size = 10)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(ingredient => ingredient.Unit)
            .OrderBy(ingredient => ingredient.Id)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ingredient>> GetAllForUser(int userId, int page = 0, int size = 10)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(ingredient => ingredient.Unit)
            .Where(ingredient => ingredient.Recipe.UserId == userId)
            .OrderBy(ingredient => ingredient.Id)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<Ingredient?> GetById(int id)
    {
        return await _dbSet
            .Include(ingredient => ingredient.Unit)
            .FirstOrDefaultAsync(ingredient => ingredient.Id == id);
    }

    public async Task<Ingredient?> GetByIdForUser(int id, int userId)
    {
        return await _dbSet
            .Include(ingredient => ingredient.Unit)
            .FirstOrDefaultAsync(ingredient => ingredient.Id == id && ingredient.Recipe.UserId == userId);
    }

    public async Task<Ingredient?> Create(Ingredient ingredient)
    {
        await _dbSet.AddAsync(ingredient);
        await context.SaveChangesAsync();
        return ingredient;
    }

    public async Task<Ingredient?> Update(Ingredient ingredient)
    {
        _dbSet.Update(ingredient);
        await context.SaveChangesAsync();
        return ingredient;
    }
}
