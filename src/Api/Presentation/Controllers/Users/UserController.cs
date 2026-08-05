using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Api.Domain.DTOs.Users;
using Recipes.Api.Domain.Entities.Enums;
using Recipes.Api.Domain.Exceptions;
using Recipes.Api.Domain.Interfaces.Users;

namespace Recipes.Api.Presentation.Controllers.Users;

[ApiController]
[Authorize(Roles = nameof(Roles.ADMIN))]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("")]
    public async Task<ActionResult<UserResponse>> GetCurrent()
    {
        var user = await userService.GetCurrent();

        return user == null
            ? throw new NotFoundException("Current user was not found.")
            : Ok(user);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetById(int id)
    {
        var user = await userService.GetById(id);

        return user == null
            ? throw new NotFoundException($"User with id '{id}' was not found.")
            : Ok(user);
    }

    [HttpPost("")]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var createdUser = await userService.Create(request);

        return createdUser == null
            ? throw new BadRequestException("Failed to create user.")
            : Ok(createdUser);
    }

    [HttpPut("")]
    public async Task<ActionResult<UserResponse>> Update([FromBody] UpdateUserRequest request)
    {
        var updatedUser = await userService.Update(request);

        return updatedUser == null
            ? throw new NotFoundException($"User with id '{request.Id}' was not found.")
            : Ok(updatedUser);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return !await userService.Disable(id) ? throw new NotFoundException($"User with id '{id}' was not found.") : NoContent();
    }
}