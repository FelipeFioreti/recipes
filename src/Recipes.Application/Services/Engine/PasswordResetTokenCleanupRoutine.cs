using Recipes.Application.Interfaces.Engine;
using Recipes.Domain.Interfaces.Token;

namespace Recipes.Application.Services.Engine;

public class PasswordResetTokenCleanupRoutine(IPasswordResetTokenRepository passwordResetTokenRepository)
    : IPasswordResetTokenCleanupRoutine
{
    public async Task Execute(CancellationToken cancellationToken = default)
    {
        await passwordResetTokenRepository.DeleteExpired(DateTime.UtcNow, cancellationToken);
    }
}
