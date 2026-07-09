using Microsoft.EntityFrameworkCore;
using Recipes.Api.Domain.Entities.Admin;
using Recipes.Api.Domain.Interfaces.Recipes;
using Recipes.Api.Infrastructure.Data.Context;

namespace Recipes.Api.Infrastructure.Repositories;

public class UnitRepository(ApplicationDbContext context) : IUnitRepository
{
    private readonly DbSet<Unit> _dbSet = context.Units;

    public async Task<IEnumerable<Unit>> GetAll(int page = 0, int size = 10)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(unit => unit.Id)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<Unit?> GetById(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<Unit?> Create(Unit unit)
    {
        await _dbSet.AddAsync(unit);
        await context.SaveChangesAsync();
        return unit;
    }

    public async Task<Unit?> Update(Unit unit)
    {
        _dbSet.Update(unit);
        await context.SaveChangesAsync();
        return unit;
    }
}
