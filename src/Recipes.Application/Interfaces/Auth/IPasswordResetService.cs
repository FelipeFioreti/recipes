using Recipes.Application.DTOs.Auth;

namespace Recipes.Application.Interfaces.Auth;

public interface IPasswordResetService
{
    Task RequestReset(ForgotPasswordRequest request);
    Task<bool> ResetPassword(ResetPasswordRequest request);
}
