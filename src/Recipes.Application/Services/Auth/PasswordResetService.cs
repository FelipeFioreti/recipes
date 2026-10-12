using System.Buffers.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Recipes.Application.DTOs.Auth;
using Recipes.Application.Interfaces.Auth;
using Recipes.Application.Interfaces.Email;
using Recipes.Application.Settings;
using Recipes.Domain.Entities.Token;
using Recipes.Domain.Entities.Users;
using Recipes.Domain.Interfaces.Token;
using Recipes.Domain.Interfaces.Users;

namespace Recipes.Application.Services.Auth;

public class PasswordResetService(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordService passwordService,
    IEmailService emailService,
    IOptions<AppSettings> appSettings,
    ILogger<PasswordResetService> logger) : IPasswordResetService
{
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task RequestReset(ForgotPasswordRequest request)
    {
        logger.LogDebug("RequestReset()");

        var user = await userRepository.GetByEmail(request.Email.ToLower());

        if (user == null)
            return;

        await passwordResetTokenRepository.DeleteByUser(user.Id);

        var token = GenerateToken();
        await passwordResetTokenRepository.Create(new PasswordResetToken(
            TokenHasher.Hash(token),
            DateTime.UtcNow.AddMinutes(_appSettings.PasswordResetTokenExpirationMinutes),
            user));

        var resetLink = $"{_appSettings.PasswordResetUrl}?token={token}";

        await TrySendEmail(
            user,
            PasswordResetEmails.ResetSubject,
            PasswordResetEmails.ResetBody(user.Name, resetLink, _appSettings.PasswordResetTokenExpirationMinutes));
    }

    public async Task<bool> ResetPassword(ResetPasswordRequest request)
    {
        logger.LogDebug("ResetPassword()");

        var resetToken = await passwordResetTokenRepository.GetByHash(TokenHasher.Hash(request.Token));

        if (resetToken == null || !resetToken.IsValid())
            return false;

        var user = await userRepository.GetById(resetToken.UserId);

        if (user == null)
            return false;

        resetToken.MarkUsed();
        await passwordResetTokenRepository.Update(resetToken);

        user.ChangePasswordHash(passwordService.HashPassword(request.NewPassword));
        await userRepository.Update(user);

        await passwordResetTokenRepository.DeleteByUser(user.Id);
        await refreshTokenRepository.DeleteByUser(user.Id);

        await TrySendEmail(user, PasswordResetEmails.ChangedSubject, PasswordResetEmails.ChangedBody(user.Name));

        return true;
    }

    private async Task TrySendEmail(User user, string subject, string htmlBody)
    {
        try
        {
            await emailService.Send(user.Email, subject, htmlBody);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to send password reset email to user {UserId}.", user.Id);
        }
    }

    private static string GenerateToken()
    {
        return Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(32));
    }
}
