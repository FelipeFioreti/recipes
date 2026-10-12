using Quartz;
using Recipes.Application.Interfaces.Engine;

namespace Recipes.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class PasswordResetTokenCleanupJob(IEngineService engineService, ILogger<PasswordResetTokenCleanupJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await engineService.CleanupExpiredPasswordResetTokens(context.CancellationToken);
        }
        catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to execute password reset token cleanup job.");
            throw;
        }
    }
}
