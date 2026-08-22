using Microsoft.EntityFrameworkCore;
using Recipes.Api.Domain.Entities.Token;
using Recipes.Api.Domain.Interfaces.Token;
using Recipes.Api.Infrastructure.Data.Context;

namespace Recipes.Api.Infrastructure.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
{
    private readonly DbSet<RefreshToken> _dbSet = context.Set<RefreshToken>();

    public async Task<RefreshToken?> GetTokenByUserId(int userId)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.UserId == userId);
    }

    public async Task Delete(RefreshToken token)
    {
        _dbSet.Remove(token);
        await context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> Create(RefreshToken token)
    {
        await _dbSet.AddAsync(token);
        await context.SaveChangesAsync();
        return token;
    }
}