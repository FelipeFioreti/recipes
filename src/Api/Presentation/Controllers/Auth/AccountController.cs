using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Api.Domain.DTOs.Auth;
using Recipes.Api.Domain.Exceptions;
using Recipes.Api.Domain.Interfaces.Auth;

namespace Recipes.Api.Presentation.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AccountController(IAuthService authService) : ControllerBase
{
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest authenticateRequest)
    {
        var response = await authService.Authenticate(authenticateRequest);

        if (response == null)
            throw new UnauthorizedException("Username or password is incorrect.");

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerUserRequest)
    {
        var response = await authService.Register(registerUserRequest);

        if (response == null)
            throw new BadRequestException("Failed to register user.");

        return Ok(response);
    }
}
