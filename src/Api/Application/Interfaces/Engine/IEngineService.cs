namespace Recipes.Api.Application.Interfaces.Engine;

public interface IEngineService
{
    Task CleanupExpiredRefreshTokens(CancellationToken cancellationToken = default);
}
