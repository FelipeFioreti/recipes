using Microsoft.EntityFrameworkCore;
using Recipes.Api.Domain.Entities.Recipes;
using Recipes.Api.Domain.Interfaces.Recipes;
using Recipes.Api.Infrastructure.Data.Context;

namespace Recipes.Api.Infrastructure.Repositories;

public class StepRepository(ApplicationDbContext context) : IStepRepository
{
    private readonly DbSet<Step> _dbSet = context.Steps;

    public async Task<IEnumerable<Step>> GetAll(int page = 0, int size = 10)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(step => step.Id)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<IEnumerable<Step>> GetAllForUser(int userId, int page = 0, int size = 10)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(step => step.Recipe.UserId == userId)
            .OrderBy(step => step.Id)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<Step?> GetById(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(step => step.Id == id);
    }

    public async Task<Step?> GetByIdForUser(int id, int userId)
    {
        return await _dbSet.FirstOrDefaultAsync(step => step.Id == id && step.Recipe.UserId == userId);
    }

    public async Task<Step?> Create(Step step)
    {
        await _dbSet.AddAsync(step);
        await context.SaveChangesAsync();
        return step;
    }

    public async Task<Step?> Update(Step step)
    {
        _dbSet.Update(step);
        await context.SaveChangesAsync();
        return step;
    }
}
