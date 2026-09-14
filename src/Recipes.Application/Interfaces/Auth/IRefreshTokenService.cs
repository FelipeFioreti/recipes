using Recipes.Domain.Entities.Token;
using Recipes.Domain.Entities.Users;

namespace Recipes.Application.Interfaces.Auth;

public interface IRefreshTokenService
{
    Task<string?> Create(User user);
    Task<RefreshToken?> Update(RefreshToken token);
    Task<RefreshToken?> GetByHash(string token);
    Task<bool> Delete(string token);
    Task DeleteByUser(int userId);
    Task<RotateRefreshTokenResult?> Rotate(string token);
}
