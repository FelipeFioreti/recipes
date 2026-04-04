using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Presentation.Controllers.Recipes;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RecipeController(IRecipeService recipeService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<Recipe?> GetById(int id)
    {
        return await recipeService.GetById(id);
    }

    [HttpGet]
    public async Task<IEnumerable<Recipe>> GetAll()
    {
        return await recipeService.GetAll();
    }

    [HttpPost]
    public async Task<Recipe?> Create([FromBody] Recipe recipe)
    {
        return await recipeService.Create(recipe);
    }

    [HttpPut]
    public async Task<Recipe?> Update([FromBody] Recipe recipe)
    {
        return await recipeService.Update(recipe);
    }

    [HttpDelete]
    public async Task Disable(int id)
    {
        await recipeService.Disable(id);
    }
}
