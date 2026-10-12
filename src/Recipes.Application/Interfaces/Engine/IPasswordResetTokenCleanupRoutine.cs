namespace Recipes.Application.Interfaces.Engine;

public interface IPasswordResetTokenCleanupRoutine
{
    Task Execute(CancellationToken cancellationToken = default);
}
