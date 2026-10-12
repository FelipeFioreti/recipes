using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Recipes.Application.DTOs.Auth;
using Recipes.Application.Interfaces.Auth;
using Recipes.Domain.Exceptions;

namespace Recipes.Api.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AccountController(IAuthService authService, IPasswordResetService passwordResetService) : ControllerBase
{
    public const string PasswordResetRateLimitPolicy = "password-reset";

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthenticateRequest authenticateRequest)
    {
        var response = await authService.Login(authenticateRequest);

        return response != null ? Ok(response) : throw new UnauthorizedException("Username or password is incorrect.");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest logoutRequest)
    {
        var response = await authService.Logout(logoutRequest);

        return !response ? throw new UnauthorizedException("Refresh token is invalid.") : NoContent();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerUserRequest)
    {
        var response = await authService.Register(registerUserRequest);

        return response == null ? throw new BadRequestException("Failed to register user.") : Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var response = await authService.RefreshToken(refreshTokenRequest);

        return response == null ? throw new UnauthorizedException("Refresh token is invalid or expired.") : Ok(response);
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting(PasswordResetRateLimitPolicy)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest)
    {
        await passwordResetService.RequestReset(forgotPasswordRequest);

        return NoContent();
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting(PasswordResetRateLimitPolicy)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
    {
        var response = await passwordResetService.ResetPassword(resetPasswordRequest);

        return !response ? throw new BadRequestException("Reset token is invalid or expired.") : NoContent();
    }
}

