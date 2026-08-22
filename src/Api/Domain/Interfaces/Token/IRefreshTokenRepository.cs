using Recipes.Api.Domain.Entities.Token;

namespace Recipes.Api.Domain.Interfaces.Token;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetTokenByUserId(int userId);
    Task Delete(RefreshToken token);
    Task<RefreshToken?> Create(RefreshToken token);
}