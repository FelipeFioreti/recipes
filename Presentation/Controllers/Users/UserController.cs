using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Domain.Constants;
using Recipes.Domain.DTOs.Users;
using Recipes.Domain.Interfaces.Auth;
using Recipes.Domain.Interfaces.Users;

namespace Recipes.Presentation.Controllers.Users;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/[controller]")]
public class UserController(IUserService userService, IUserContext userContext) : ControllerBase
{
    [HttpGet("")]
    public async Task<ActionResult<UserResponse>> GetCurrent()
    {
        if (userContext.UserId is not { } userId)
            return Unauthorized();

        var user = await userService.GetById(userId);

        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost("")]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        if (userContext.UserId is not { } userId)
            return Unauthorized();

        var createdUser = await userService.Create(request);

        return createdUser == null ? BadRequest() : Ok(createdUser);
    }

    [HttpPut("")]
    public async Task<ActionResult<UserResponse>> Update([FromBody] UpdateUserRequest request)
    {
        if (userContext.UserId is not { } userId)
            return Unauthorized();

        var updatedUser = await userService.Update(userId, request);

        return updatedUser == null ? BadRequest() : Ok(updatedUser);
    }

    [HttpDelete("")]
    public async Task<IActionResult> Delete()
    {
        if (userContext.UserId is not { } userId)
            return Unauthorized();

        var disabled = await userService.Disable(userId);

        return disabled ? NoContent() : NotFound();
    }
}
