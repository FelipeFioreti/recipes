using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Domain.Constants;
using Recipes.Domain.DTOs.Recipes;
using Recipes.Domain.Exceptions;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Presentation.Controllers.Recipes;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AuthenticatedUser)]
[Route("api/[controller]")]
public class RecipeController(IRecipeService recipeService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RecipeResponse>> GetById(int id)
    {
        var recipe = await recipeService.GetById(id);

        return recipe == null
            ? throw new NotFoundException($"Recipe with id '{id}' was not found.")
            : Ok(recipe);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeResponse>>> GetAll()
    {
        var recipes = await recipeService.GetAll();

        return Ok(recipes);
    }

    [HttpPost]
    public async Task<ActionResult<RecipeResponse>> Create([FromBody] CreateRecipeRequest request)
    {
        var createdRecipe = await recipeService.Create(request);

        return createdRecipe == null
            ? throw new BadRequestException("Failed to create recipe.")
            : Ok(createdRecipe);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RecipeResponse>> Update(int id, [FromBody] UpdateRecipeRequest request)
    {
        var updatedRecipe = await recipeService.Update(id, request);

        return updatedRecipe == null
            ? throw new NotFoundException($"Recipe with id '{id}' was not found.")
            : Ok(updatedRecipe);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Disable(int id)
    {
        var disabled = await recipeService.Disable(id);

        if (!disabled)
            throw new NotFoundException($"Recipe with id '{id}' was not found.");

        return NoContent();
    }
}
