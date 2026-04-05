using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Domain.Constants;
using Recipes.Domain.DTOs.Users;
using Recipes.Domain.Interfaces.Users;

namespace Recipes.Presentation.Controllers.Users;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("")]
    public async Task<ActionResult<UserResponse>> GetCurrent()
    {
        var user = await userService.GetCurrent();

        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost("")]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var createdUser = await userService.Create(request);

        return createdUser == null ? BadRequest() : Ok(createdUser);
    }

    [HttpPut("")]
    public async Task<ActionResult<UserResponse>> Update([FromBody] UpdateUserRequest request)
    {
        var updatedUser = await userService.UpdateCurrent(request);

        return updatedUser == null ? BadRequest() : Ok(updatedUser);
    }

    [HttpDelete("")]
    public async Task<IActionResult> Delete()
    {
        var disabled = await userService.DisableCurrent();

        return disabled ? NoContent() : NotFound();
    }
}
