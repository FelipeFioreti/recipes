using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Api.Application.Interfaces.Auth;
using Recipes.Api.Domain.DTOs.Auth;
using Recipes.Api.Domain.Exceptions;
using Recipes.Api.Domain.Interfaces.Auth;

namespace Recipes.Api.Presentation.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AccountController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthenticateRequest authenticateRequest)
    {
        var response = await authService.Login(authenticateRequest);

        return response == null ? throw new UnauthorizedException("Username or password is incorrect.") : Ok(response);
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
}

