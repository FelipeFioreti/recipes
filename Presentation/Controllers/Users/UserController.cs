using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Domain.DTOs.Users;
using Recipes.Domain.Entities.Users;
using Recipes.Domain.Interfaces.Users;

namespace Recipes.Presentation.Controllers.Users;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> Get(int id)
    {
        var user = await userService.GetById(id);

        if (user == null)
            return NotFound();

        return Ok(new UserResponse(user));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        var users = await userService.GetAll();

        return Ok(users.Select(user => new UserResponse(user)));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password
        };

        var createdUser = await userService.Create(user);

        if (createdUser == null)
            return BadRequest();

        return Ok(new UserResponse(createdUser));
    }

    [HttpPut]
    public async Task<ActionResult<UserResponse>> Update([FromBody] UpdateUserRequest request)
    {
        var existingUser = await userService.GetById(request.Id);

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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await userService.Disable(id);

        return NoContent();
    }
}
