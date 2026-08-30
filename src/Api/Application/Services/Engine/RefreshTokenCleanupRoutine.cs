using Recipes.Api.Domain.Interfaces.Token;

namespace Recipes.Api.Application.Services.Engine;

public class RefreshTokenCleanupRoutine(IRefreshTokenRepository refreshTokenRepository) : IRefreshTokenCleanupRoutine
{
    public async Task Execute(CancellationToken cancellationToken = default)
    {
        await refreshTokenRepository.DeleteExpired(DateTime.UtcNow, cancellationToken);
    }
}
