namespace Recipes.Application.Interfaces.Engine;

public interface IEngineService
{
    Task CleanupExpiredRefreshTokens(CancellationToken cancellationToken = default);
}
