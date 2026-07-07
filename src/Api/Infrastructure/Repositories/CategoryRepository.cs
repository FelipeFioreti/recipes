using Microsoft.EntityFrameworkCore;
using Recipes.Api.Domain.Entities.Recipes;
using Recipes.Api.Domain.Interfaces.Recipes;
using Recipes.Api.Infrastructure.Data.Context;

namespace Recipes.Api.Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
{
    private readonly DbSet<Category> _dbSet = context.Categories;

    public async Task<IEnumerable<Category>> GetAll(int page = 0, int size = 10)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<Category?> GetById(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<Category?> Create(Category category)
    {
        await _dbSet.AddAsync(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> Update(Category category)
    {
        _dbSet.Update(category);
        await context.SaveChangesAsync();
        return category;
    }
}
