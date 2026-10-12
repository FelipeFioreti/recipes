using Recipes.Application.Interfaces.Engine;
using Recipes.Domain.Interfaces.Token;

namespace Recipes.Application.Services.Engine;

public class EngineService(
    IRefreshTokenCleanupRoutine refreshTokenCleanupRoutine,
    IPasswordResetTokenCleanupRoutine passwordResetTokenCleanupRoutine) : IEngineService
{
    public async Task CleanupExpiredRefreshTokens(CancellationToken cancellationToken = default)
    {
        await refreshTokenCleanupRoutine.Execute(cancellationToken);
    }

    public async Task CleanupExpiredPasswordResetTokens(CancellationToken cancellationToken = default)
    {
        await passwordResetTokenCleanupRoutine.Execute(cancellationToken);
    }
}
