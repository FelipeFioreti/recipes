using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Api.Domain.DTOs.Recipes;
using Recipes.Api.Domain.Entities.Enums;
using Recipes.Api.Domain.Exceptions;
using Recipes.Api.Domain.Interfaces.Recipes;

namespace Recipes.Api.Presentation.Controllers.Recipes;

[ApiController]
[Authorize(Roles = nameof(Roles.USER))]
[Route("api/[controller]")]
public class IngredientController(IIngredientService ingredientService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<IngredientResponse>> GetById(int id)
    {
        var ingredient = await ingredientService.GetById(id);

        return ingredient == null
            ? throw new NotFoundException($"Ingredient with id '{id}' was not found.")
            : Ok(ingredient);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IngredientResponse>>> GetAll([FromQuery] int page = 0,
        [FromQuery] int size = 10)
    {
        return Ok(await ingredientService.GetAll(page, size));
    }

    [HttpPost]
    public async Task<ActionResult<IngredientResponse>> Create([FromBody] CreateIngredientRequest request)
    {
        var ingredient = await ingredientService.Create(request);

        return ingredient == null
            ? throw new BadRequestException("Failed to create ingredient.")
            : Ok(ingredient);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<IngredientResponse>> Update(int id, [FromBody] UpdateIngredientRequest request)
    {
        var ingredient = await ingredientService.Update(id, request);

        return ingredient == null
            ? throw new NotFoundException($"Ingredient with id '{id}' was not found.")
            : Ok(ingredient);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Disable(int id)
    {
        var disabled = await ingredientService.Disable(id);

        if (!disabled)
            throw new NotFoundException($"Ingredient with id '{id}' was not found.");

        return NoContent();
    }
}
