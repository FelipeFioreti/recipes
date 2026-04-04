using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Domain.Constants;
using Recipes.Domain.DTOs.Recipes;
using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Auth;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Presentation.Controllers.Recipes;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AuthenticatedUser)]
[Route("api/[controller]")]
public class RecipeController(IRecipeService recipeService, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Recipe>> GetById(int id)
    {
        if (currentUser.UserId is not { } userId)
            return Unauthorized();

        var recipe = await recipeService.GetById(id, userId);

        return recipe == null ? NotFound() : Ok(recipe);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetAll()
    {
        if (currentUser.UserId is not { } userId)
            return Unauthorized();

        var recipes = await recipeService.GetAll(userId);

        return Ok(recipes);
    }

    [HttpPost]
    public async Task<ActionResult<Recipe>> Create([FromBody] CreateRecipeRequest request)
    {
        if (currentUser.UserId is not { } userId)
            return Unauthorized();

        var recipe = new Recipe
        {
            Name = request.Name,
            Description = request.Description,
            RecipeTypeId = request.RecipeTypeId
        };

        var createdRecipe = await recipeService.Create(userId, recipe);

        return createdRecipe == null ? BadRequest() : Ok(createdRecipe);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Recipe>> Update(int id, [FromBody] UpdateRecipeRequest request)
    {
        if (currentUser.UserId is not { } userId)
            return Unauthorized();

        var recipe = new Recipe
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            RecipeTypeId = request.RecipeTypeId
        };

        var updatedRecipe = await recipeService.Update(userId, recipe);

        return updatedRecipe == null ? NotFound() : Ok(updatedRecipe);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Disable(int id)
    {
        if (currentUser.UserId is not { } userId)
            return Unauthorized();

        var disabled = await recipeService.Disable(id, userId);

        return disabled ? NoContent() : NotFound();
    }
}
