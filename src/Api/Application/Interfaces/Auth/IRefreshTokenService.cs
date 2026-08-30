using Recipes.Api.Domain.Entities.Token;
using Recipes.Api.Domain.Entities.Users;

namespace Recipes.Api.Application.Interfaces.Auth;

public interface IRefreshTokenService
{
    Task<string?> Create(User user);
    Task<RefreshToken?> Update(RefreshToken token);
    Task<RefreshToken?> GetByHash(string token);
    Task<bool> Delete(string token);
    Task DeleteByUser(int userId);
    Task<RotateRefreshTokenResult?> Rotate(string token);
}
