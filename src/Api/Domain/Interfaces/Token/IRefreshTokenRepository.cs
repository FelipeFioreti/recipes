using Recipes.Api.Domain.Entities.Token;

namespace Recipes.Api.Domain.Interfaces.Token;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> Create(RefreshToken token);
    Task<RefreshToken?> Update(RefreshToken token);
    Task<RefreshToken?> GetByHash(string token);
    Task Delete(RefreshToken token);
    Task DeleteByUser(int userId);
    Task DeleteExpired(DateTime now, CancellationToken cancellationToken = default);
}
