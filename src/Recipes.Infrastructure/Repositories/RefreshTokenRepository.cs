using Microsoft.EntityFrameworkCore;
using Recipes.Domain.Entities.Token;
using Recipes.Domain.Interfaces.Token;
using Recipes.Infrastructure.Data.Context;
using UserEntity = Recipes.Domain.Entities.Users.User;

namespace Recipes.Infrastructure.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
{
    private readonly DbSet<RefreshToken> _dbSet = context.RefreshTokens;
    
    public async Task<RefreshToken?> Create(RefreshToken token)
    {
        await _dbSet.AddAsync(token);
        await context.SaveChangesAsync();
        return token;
    }
    public async Task<RefreshToken?> Update(RefreshToken refreshToken)
    {
        _dbSet.Update(refreshToken);
        await context.SaveChangesAsync();
        return refreshToken;
    }
    
    public async Task<RefreshToken?> GetByHash(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.TokenHash == token);
    }   
    
    public async Task Delete(RefreshToken token)
    {
        _dbSet.Remove(token);
        await context.SaveChangesAsync();
    }

    public async Task DeleteByUser(int userId)
    {
        await _dbSet.Where(t => t.UserId == userId).ExecuteDeleteAsync();
    }

    public async Task DeleteExpired(DateTime now, CancellationToken cancellationToken = default)
    {
        await _dbSet.IgnoreQueryFilters().Where(t => t.ExpiresAt <= now).ExecuteDeleteAsync(cancellationToken);
    }
}
