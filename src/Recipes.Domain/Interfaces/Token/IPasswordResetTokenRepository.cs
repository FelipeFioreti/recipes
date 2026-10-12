using Recipes.Domain.Entities.Token;

namespace Recipes.Domain.Interfaces.Token;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> Create(PasswordResetToken token);
    Task<PasswordResetToken?> Update(PasswordResetToken token);
    Task<PasswordResetToken?> GetByHash(string tokenHash);
    Task DeleteByUser(int userId);
    Task DeleteExpired(DateTime now, CancellationToken cancellationToken = default);
}
