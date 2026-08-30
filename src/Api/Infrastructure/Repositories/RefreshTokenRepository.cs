using Microsoft.EntityFrameworkCore;
using Recipes.Api.Domain.Entities.Token;
using Recipes.Api.Domain.Interfaces.Token;
using Recipes.Api.Infrastructure.Data.Context;
using UserEntity = Recipes.Api.Domain.Entities.Users.User;

namespace Recipes.Api.Infrastructure.Repositories;

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
