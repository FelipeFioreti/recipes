using Recipes.Application.Interfaces.Engine;
using Recipes.Domain.Interfaces.Token;

namespace Recipes.Application.Services.Engine;

public class EngineService(IRefreshTokenCleanupRoutine refreshTokenCleanupRoutine) : IEngineService
{
    public async Task CleanupExpiredRefreshTokens(CancellationToken cancellationToken = default)
    {
        await refreshTokenCleanupRoutine.Execute(cancellationToken);
    }
}
