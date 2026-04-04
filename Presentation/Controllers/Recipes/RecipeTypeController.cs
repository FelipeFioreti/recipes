using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Presentation.Controllers.Recipes;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RecipeTypeController(IRecipeTypeService recipeTypeService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<RecipeType?> Get(int id)
    {
        return await recipeTypeService.GetById(id);
    }

    [HttpGet]
    public async Task<IEnumerable<RecipeType>> GetAll()
    {
        return await recipeTypeService.GetAll();
    }

    [HttpPost]
    public async Task<RecipeType?> Create([FromBody] RecipeType recipeType)
    {
        return await recipeTypeService.Create(recipeType);
    }

    [HttpPut]
    public async Task<RecipeType?> Update([FromBody] RecipeType recipeType)
    {
        return await recipeTypeService.Update(recipeType);
    }

    [HttpDelete]
    public async Task Delete(int id)
    {
        await recipeTypeService.Disable(id);
    }
}
