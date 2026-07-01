using Microsoft.EntityFrameworkCore;
using Recipes.Api.Domain.Entities.Users;
using Recipes.Api.Domain.Interfaces.Users;
using Recipes.Api.Infrastructure.Data.Context;

namespace Recipes.Api.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    private readonly DbSet<User> _dbSet = context.Users;

    public async Task<IEnumerable<User>> GetAll(int page = 1, int size = 10)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<User?> GetById(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> Create(User user)
    {
        await _dbSet.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> Update(User user)
    {
        _dbSet.Update(user);
        await context.SaveChangesAsync();
        return user;
    }
}