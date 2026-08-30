namespace Recipes.Api.Application.Services.Engine;

public interface IRefreshTokenCleanupRoutine
{
    Task Execute(CancellationToken cancellationToken = default);
}
