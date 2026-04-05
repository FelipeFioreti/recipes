using Recipes.Domain.DTOs.Recipes;
using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Application.Services.Recipes;

public class RecipeService(IRecipeRepository recipeRepository, ILogger<RecipeService> logger) : IRecipeService
{
    public async Task<IEnumerable<RecipeResponse>> GetAll(int userId)
    {
        logger.LogDebug("GetAll()");

        var recipes = await recipeRepository.GetAll(userId);

        return recipes.Select(ToResponse);
    }

    public async Task<RecipeResponse?> GetById(int id, int userId)
    {
        logger.LogDebug("GetById()");

        var recipe = await recipeRepository.GetById(id, userId);

        return recipe == null ? null : ToResponse(recipe);
    }

    public async Task<RecipeResponse?> Create(int userId, CreateRecipeRequest request)
    {
        logger.LogDebug("Create()");

        var recipe = await recipeRepository.Create(
            new Recipe(request.Name, request.Description, request.RecipeTypeId, userId));

        return recipe == null ? null : ToResponse(recipe);
    }

    public async Task<RecipeResponse?> Update(int userId, UpdateRecipeRequest request)
    {
        logger.LogDebug("Update()");

        var existingRecipe = await recipeRepository.GetById(request.Id, userId);

        if (existingRecipe == null)
            return null;

        existingRecipe.Update(request);

        var updatedRecipe = await recipeRepository.Update(existingRecipe);

        return updatedRecipe == null ? null : ToResponse(updatedRecipe);
    }

    public async Task<bool> Disable(int id, int userId)
    {
        logger.LogDebug("Disable()");

        var recipe = await recipeRepository.GetById(id, userId);

        if (recipe == null)
            return false;

        recipe.Disable();
        await recipeRepository.Update(recipe);

        return true;
    }

    public async Task<bool> Delete(int id, int userId)
    {
        logger.LogDebug("Delete()");

        var recipe = await recipeRepository.GetById(id, userId);

        if (recipe == null)
            return false;

        await recipeRepository.Delete(recipe);

        return true;
    }

    private static RecipeResponse ToResponse(Recipe recipe)
    {
        return new RecipeResponse(recipe);
    }
}
