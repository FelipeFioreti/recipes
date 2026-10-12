using Microsoft.EntityFrameworkCore;
using Recipes.Domain.Entities.Token;
using Recipes.Domain.Interfaces.Token;
using Recipes.Infrastructure.Data.Context;

namespace Recipes.Infrastructure.Repositories;

public class PasswordResetTokenRepository(ApplicationDbContext context) : IPasswordResetTokenRepository
{
    private readonly DbSet<PasswordResetToken> _dbSet = context.PasswordResetTokens;

    public async Task<PasswordResetToken?> Create(PasswordResetToken token)
    {
        await _dbSet.AddAsync(token);
        await context.SaveChangesAsync();
        return token;
    }

    public async Task<PasswordResetToken?> Update(PasswordResetToken token)
    {
        _dbSet.Update(token);
        await context.SaveChangesAsync();
        return token;
    }

    public async Task<PasswordResetToken?> GetByHash(string tokenHash)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
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
