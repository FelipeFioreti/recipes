using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Domain.Constants;
using Recipes.Domain.DTOs.Recipes;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Presentation.Controllers.Recipes;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/[controller]")]
public class RecipeTypeController(IRecipeTypeService recipeTypeService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RecipeTypeResponse>> Get(int id)
    {
        var recipeType = await recipeTypeService.GetById(id);

        return recipeType == null ? NotFound() : Ok(recipeType);
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

        return recipeType == null ? BadRequest() : Ok(recipeType);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RecipeTypeResponse>> Update(int id, [FromBody] UpdateRecipeTypeRequest request)
    {
        var recipeType = await recipeTypeService.Update(id, request);

        return recipeType == null ? NotFound() : Ok(recipeType);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Disable(int id)
    {
        var disabled = await recipeTypeService.Disable(id);

        return disabled ? NoContent() : NotFound();
    }
}
