using Microsoft.EntityFrameworkCore;
using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Recipes;
using Recipes.Infrastructure.Data.Context;

namespace Recipes.Infrastructure.Repositories;

public class RecipeTypeRepository(ApplicationDbContext context) : IRecipeTypeRepository
{
    private readonly DbSet<RecipeType> _dbSet = context.RecipeTypes;

    public async Task<IEnumerable<RecipeType>> GetAll()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<RecipeType?> GetById(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<RecipeType?> Create(RecipeType recipeType)
    {
        await _dbSet.AddAsync(recipeType);
        await context.SaveChangesAsync();
        return recipeType;
    }

    public async Task<RecipeType?> Update(RecipeType recipeType)
    {
        _dbSet.Update(recipeType);
        await context.SaveChangesAsync();
        return recipeType;
    }
}
