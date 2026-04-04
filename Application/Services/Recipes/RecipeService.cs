using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Application.Services.Recipes;

public class RecipeService(IRecipeRepository recipeRepository, ILogger<RecipeService> logger) : IRecipeService
{
    public async Task<IEnumerable<Recipe>> GetAll()
    {
        logger.LogDebug("GetAll()");

        return await recipeRepository.GetAll();
    }

    public async Task<Recipe?> GetById(int id)
    {
        logger.LogDebug("GetById()");

        return await recipeRepository.GetById(id);
    }

    public async Task<Recipe?> Create(Recipe recipe)
    {
        logger.LogDebug("Create()");

        return await recipeRepository.Create(recipe);
    }

    public async Task<Recipe?> Update(Recipe recipe)
    {
        logger.LogDebug("Update()");

        return await recipeRepository.Update(recipe);
    }

    public async Task Disable(int id)
    {
        logger.LogDebug("Disable()");

        var recipe = await GetById(id);

        if (recipe == null)
            return;

        recipe.DeletedAt = DateTime.Now;
        await recipeRepository.Update(recipe);
    }

    public async Task Delete(int id)
    {
        logger.LogDebug("Delete()");

        var recipe = await GetById(id);

        if (recipe == null)
            return;

        await recipeRepository.Delete(recipe);
    }
}
