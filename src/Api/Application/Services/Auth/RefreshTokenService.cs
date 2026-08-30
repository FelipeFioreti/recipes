using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Recipes.Api.Application.Interfaces.Auth;
using Recipes.Api.Domain.Entities.Settings;
using Recipes.Api.Domain.Entities.Token;
using Recipes.Api.Domain.Entities.Users;
using Recipes.Api.Domain.Interfaces.Token;
using Recipes.Api.Domain.Interfaces.Users;

namespace Recipes.Api.Application.Services.Auth;

public class RefreshTokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IOptions<AppSettings> appSettings,
    ILogger<RefreshTokenService> logger) : IRefreshTokenService
{
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<string?> Create(User user)
    {
        logger.LogDebug("Create()");

        var refreshToken = GenerateRefreshToken();
        var token = await refreshTokenRepository.Create(new RefreshToken(
            HashToken(refreshToken),
            DateTime.UtcNow.AddDays(_appSettings.RefreshTokenExpirationDays),
            user));

        return token == null ? null : refreshToken;
    }

    public async Task<RefreshToken?> Update(RefreshToken token)
    {
        logger.LogDebug("Update()");
        
        return await refreshTokenRepository.Update(token);
    }

    public async Task<RefreshToken?> GetByHash(string token)
    {
        logger.LogDebug("GetByHash()");

        return await refreshTokenRepository.GetByHash(HashToken(token));
    }
    
    public async Task<bool> Delete(string token)
    {
        logger.LogDebug("Delete()");

        var refreshToken = await refreshTokenRepository.GetByHash(HashToken(token));

        if (refreshToken == null)
            return false;

        await refreshTokenRepository.Delete(refreshToken);

        return true;
    }

    public async Task DeleteByUser(int userId)
    {
        logger.LogDebug("DeleteByUser()");

        await refreshTokenRepository.DeleteByUser(userId);
    }
    
    public async Task<RotateRefreshTokenResult?> Rotate(string token)
    {
        logger.LogDebug("Rotate()");

        var refreshToken = await refreshTokenRepository.GetByHash(HashToken(token));
        
        if (refreshToken == null)
            return null;

        if (refreshToken.IsRevoked())
        {
            await DeleteByUser(refreshToken.UserId);
            return null;
        }

        if (refreshToken.IsExpired())
        {
            await refreshTokenRepository.Delete(refreshToken);
            return null;
        }

        refreshToken.Revoke();

        var user = await userRepository.GetById(refreshToken.UserId);

        if (user == null)
            return null;

        var newRefreshToken = await Create(user);

        return newRefreshToken == null ? null : new RotateRefreshTokenResult(user, newRefreshToken);
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private static string HashToken(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);
        return Convert.ToBase64String(hashBytes);
    }
}
