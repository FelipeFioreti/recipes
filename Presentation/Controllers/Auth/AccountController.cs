using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Domain.DTOs.Auth;
using Recipes.Domain.Interfaces.Auth;

namespace Recipes.Presentation.Controllers.Auth;

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
            return BadRequest(new { message = "Username or password is incorrect" });

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerUserRequest)
    {
        var response = await authService.RegisterUser(registerUserRequest);

        if (response == null)
            return BadRequest(new { message = "Failed to register user" });

        return Ok(response);
    }
}