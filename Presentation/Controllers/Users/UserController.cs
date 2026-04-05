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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetById(int id)
    {
        var user = await userService.GetById(id);

        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost("")]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var createdUser = await userService.Create(request);

        return createdUser == null ? BadRequest() : Ok(createdUser);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponse>> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var updatedUser = await userService.Update(id, request);

        return updatedUser == null ? NotFound() : Ok(updatedUser);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var disabled = await userService.Disable(id);

        return disabled ? NoContent() : NotFound();
    }
}
