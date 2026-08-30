using Recipes.Api.Application.Interfaces.Engine;
using Recipes.Api.Domain.Interfaces.Token;

namespace Recipes.Api.Application.Services.Engine;

public class EngineService(IRefreshTokenCleanupRoutine refreshTokenCleanupRoutine) : IEngineService
{
    public async Task CleanupExpiredRefreshTokens(CancellationToken cancellationToken = default)
    {
        await refreshTokenCleanupRoutine.Execute(cancellationToken);
    }
}
