using Quartz;
using Recipes.Api.Application.Interfaces.Engine;

namespace Recipes.Api.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class RefreshTokenCleanupJob(IEngineService engineService, ILogger<RefreshTokenCleanupJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await engineService.CleanupExpiredRefreshTokens(context.CancellationToken);
        }
        catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to execute refresh token cleanup job.");
            throw;
        }
    }
}
