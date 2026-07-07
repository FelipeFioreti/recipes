using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Api.Domain.DTOs.Recipes;
using Recipes.Api.Domain.Entities.Enums;
using Recipes.Api.Domain.Exceptions;
using Recipes.Api.Domain.Interfaces.Recipes;

namespace Recipes.Api.Presentation.Controllers.Recipes;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [Authorize(Roles = nameof(Roles.USER))]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> Get(int id)
    {
        var category = await categoryService.GetById(id);

        return category == null
            ? throw new NotFoundException($"Category with id '{id}' was not found.")
            : Ok(category);
    }

    [Authorize(Roles = nameof(Roles.USER))]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll([FromQuery] int page = 0,
        [FromQuery] int size = 10)
    {
        return Ok(await categoryService.GetAll(page, size));
    }

    [Authorize(Roles = nameof(Roles.ADMIN))]
    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest request)
    {
        var category = await categoryService.Create(request);

        return category == null
            ? throw new BadRequestException("Failed to create category.")
            : Ok(category);
    }

    [Authorize(Roles = nameof(Roles.ADMIN))]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> Update(int id, [FromBody] UpdateCategoryRequest request)
    {
        var category = await categoryService.Update(id, request);

        return category == null
            ? throw new NotFoundException($"Category with id '{id}' was not found.")
            : Ok(category);
    }

    [Authorize(Roles = nameof(Roles.ADMIN))]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Disable(int id)
    {
        var disabled = await categoryService.Disable(id);

        return !disabled ? throw new NotFoundException($"Category with id '{id}' was not found.") : NoContent();
    }
}
