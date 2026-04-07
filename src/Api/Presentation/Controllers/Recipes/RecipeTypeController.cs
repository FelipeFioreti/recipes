using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Api.Domain.Constants;
using Recipes.Api.Domain.DTOs.Recipes;
using Recipes.Api.Domain.Exceptions;
using Recipes.Api.Domain.Interfaces.Recipes;

namespace Recipes.Api.Presentation.Controllers.Recipes;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/[controller]")]
public class RecipeTypeController(IRecipeTypeService recipeTypeService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RecipeTypeResponse>> Get(int id)
    {
        var recipeType = await recipeTypeService.GetById(id);

        return recipeType == null
            ? throw new NotFoundException($"Recipe type with id '{id}' was not found.")
            : Ok(recipeType);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeTypeResponse>>> GetAll()
    {
        return Ok(await recipeTypeService.GetAll());
    }

    [HttpPost]
    public async Task<ActionResult<RecipeTypeResponse>> Create([FromBody] CreateRecipeTypeRequest request)
    {
        var recipeType = await recipeTypeService.Create(request);

        return recipeType == null
            ? throw new BadRequestException("Failed to create recipe type.")
            : Ok(recipeType);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RecipeTypeResponse>> Update(int id, [FromBody] UpdateRecipeTypeRequest request)
    {
        var recipeType = await recipeTypeService.Update(id, request);

        return recipeType == null
            ? throw new NotFoundException($"Recipe type with id '{id}' was not found.")
            : Ok(recipeType);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Disable(int id)
    {
        var disabled = await recipeTypeService.Disable(id);

        if (!disabled)
            throw new NotFoundException($"Recipe type with id '{id}' was not found.");

        return NoContent();
    }
}
