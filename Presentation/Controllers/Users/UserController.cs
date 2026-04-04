using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Domain.Constants;
using Recipes.Domain.DTOs.Users;
using Recipes.Domain.Entities.Users;
using Recipes.Domain.Interfaces.Auth;
using Recipes.Domain.Interfaces.Users;

namespace Recipes.Presentation.Controllers.Users;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/[controller]")]
public class UserController(IUserService userService, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet("")]
    public async Task<ActionResult<UserResponse>> GetCurrent()
    {
        if (currentUser.UserId is not { } userId)
            return Unauthorized();

        var user = await userService.GetById(userId);

        if (user == null)
            return NotFound();

        return Ok(new UserResponse(user));
    }

    [HttpPost("")]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        if (currentUser.UserId is not { } userId)
            return Unauthorized();

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            Role = request.Role
        };

        var createdUser = await userService.Create(user);

        if (createdUser == null)
            return BadRequest();

        return Ok(new UserResponse(createdUser));
    }

    [HttpPut("")]
    public async Task<ActionResult<UserResponse>> Update([FromBody] UpdateUserRequest request)
    {
        if (currentUser.UserId is not { } userId)
            return Unauthorized();

        var existingUser = await userService.GetById(userId);

        if (existingUser == null)
            return NotFound();

        existingUser.Name = request.Name;
        existingUser.Email = request.Email;

        if (!string.IsNullOrWhiteSpace(request.Password))
            existingUser.Password = request.Password;

        var updatedUser = await userService.Update(existingUser);

        if (updatedUser == null)
            return BadRequest();

        return Ok(new UserResponse(updatedUser));
    }

    [HttpDelete("")]
    public async Task<IActionResult> Delete()
    {
        if (currentUser.UserId is not { } userId)
            return Unauthorized();

        var user = await userService.GetById(userId);

        if (user == null)
            return NotFound();

        await userService.Disable(userId);

        return NoContent();
    }
}
