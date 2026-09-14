using Recipes.Application.Interfaces.Engine;
using Recipes.Domain.Interfaces.Token;

namespace Recipes.Application.Services.Engine;

public class RefreshTokenCleanupRoutine(IRefreshTokenRepository refreshTokenRepository) : IRefreshTokenCleanupRoutine
{
    public async Task Execute(CancellationToken cancellationToken = default)
    {
        await refreshTokenRepository.DeleteExpired(DateTime.UtcNow, cancellationToken);
    }
}
