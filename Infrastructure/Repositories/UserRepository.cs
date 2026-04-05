using Microsoft.EntityFrameworkCore;
using Recipes.Domain.Entities.Users;
using Recipes.Domain.Interfaces.Users;
using Recipes.Infrastructure.Data.Context;

namespace Recipes.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    private readonly DbSet<User> _dbSet = context.Users;

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _dbSet.ToListAsync();
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

    public async Task Delete(User user)
    {
        _dbSet.Remove(user);
        await context.SaveChangesAsync();
    }
}