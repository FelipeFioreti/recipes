using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Application.Services.Recipes;

public class RecipeService(IRecipeRepository recipeRepository, ILogger<RecipeService> logger) : IRecipeService
{
    public async Task<IEnumerable<Recipe>> GetAll(int userId)
    {
        logger.LogDebug("GetAll()");

        return await recipeRepository.GetAll(userId);
    }

    public async Task<Recipe?> GetById(int id, int userId)
    {
        logger.LogDebug("GetById()");

        return await recipeRepository.GetById(id, userId);
    }

    public async Task<Recipe?> Create(int userId, Recipe recipe)
    {
        logger.LogDebug("Create()");

        recipe.UserId = userId;

        return await recipeRepository.Create(recipe);
    }

    public async Task<Recipe?> Update(int userId, Recipe recipe)
    {
        logger.LogDebug("Update()");

        var existingRecipe = await recipeRepository.GetById(recipe.Id, userId);

        if (existingRecipe == null)
            return null;

        existingRecipe.Name = recipe.Name;
        existingRecipe.Description = recipe.Description;
        existingRecipe.RecipeTypeId = recipe.RecipeTypeId;
        existingRecipe.UserId = userId;

        return await recipeRepository.Update(existingRecipe);
    }

    public async Task<bool> Disable(int id, int userId)
    {
        logger.LogDebug("Disable()");

        var recipe = await GetById(id, userId);

        if (recipe == null)
            return false;

        recipe.DeletedAt = DateTime.UtcNow;
        recipe.UpdatedAt = DateTime.UtcNow;
        await recipeRepository.Update(recipe);

        return true;
    }

    public async Task<bool> Delete(int id, int userId)
    {
        logger.LogDebug("Delete()");

        var recipe = await GetById(id, userId);

        if (recipe == null)
            return false;

        await recipeRepository.Delete(recipe);

        return true;
    }
}
