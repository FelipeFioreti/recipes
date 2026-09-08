namespace Recipes.Application.Interfaces.Engine;

public interface IRefreshTokenCleanupRoutine
{
    Task Execute(CancellationToken cancellationToken = default);
}
