using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Domain.Constants;
using Recipes.Domain.DTOs.Recipes;
using Recipes.Domain.Interfaces.Auth;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Presentation.Controllers.Recipes;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AuthenticatedUser)]
[Route("api/[controller]")]
public class RecipeController(IRecipeService recipeService, IUserContext userContext) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RecipeResponse>> GetById(int id)
    {
        if (userContext.UserId is not { } userId)
            return Unauthorized();

        var recipe = await recipeService.GetById(id, userId);

        return recipe == null ? NotFound() : Ok(recipe);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeResponse>>> GetAll()
    {
        if (userContext.UserId is not { } userId)
            return Unauthorized();

        var recipes = await recipeService.GetAll(userId);

        return Ok(recipes);
    }

    [HttpPost]
    public async Task<ActionResult<RecipeResponse>> Create([FromBody] CreateRecipeRequest request)
    {
        if (userContext.UserId is not { } userId)
            return Unauthorized();

        var createdRecipe = await recipeService.Create(userId, request);

        return createdRecipe == null ? BadRequest() : Ok(createdRecipe);
    }

    [HttpPut("")]
    public async Task<ActionResult<RecipeResponse>> Update([FromBody] UpdateRecipeRequest request)
    {
        if (userContext.UserId is not { } userId)
            return Unauthorized();

        var updatedRecipe = await recipeService.Update(userId, request);

        return updatedRecipe == null ? NotFound() : Ok(updatedRecipe);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Disable(int id)
    {
        if (userContext.UserId is not { } userId)
            return Unauthorized();

        var disabled = await recipeService.Disable(id, userId);

        return disabled ? NoContent() : NotFound();
    }
}
