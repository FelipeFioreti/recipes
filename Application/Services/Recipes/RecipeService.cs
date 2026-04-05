using Recipes.Domain.DTOs.Recipes;
using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Auth;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Application.Services.Recipes;

public class RecipeService(
    IRecipeRepository recipeRepository,
    IUserContext userContext,
    ILogger<RecipeService> logger) : IRecipeService
{
    public async Task<IEnumerable<RecipeResponse>> GetAll()
    {
        logger.LogDebug("GetAll()");
        
        var recipes = await recipeRepository.GetAll(userContext.GetUserId());

        return recipes.Select(ToResponse);
    }

    public async Task<RecipeResponse?> GetById(int id)
    {
        logger.LogDebug("GetById()");
        
        var recipe = await recipeRepository.GetById(id, userContext.GetUserId());

        return recipe == null ? null : ToResponse(recipe);
    }

    public async Task<RecipeResponse?> Create(CreateRecipeRequest request)
    {
        logger.LogDebug("Create()");
        
        var recipe = await recipeRepository.Create(
            new Recipe(request.Name, request.Description, request.RecipeTypeId, userContext.GetUserId()));

        return recipe == null ? null : ToResponse(recipe);
    }

    public async Task<RecipeResponse?> Update(int id, UpdateRecipeRequest request)
    {
        logger.LogDebug("Update()");

        var existingRecipe = await recipeRepository.GetById(id, userContext.GetUserId());

        if (existingRecipe == null)
            return null;

        existingRecipe.Update(request);

        var updatedRecipe = await recipeRepository.Update(existingRecipe);

        return updatedRecipe == null ? null : ToResponse(updatedRecipe);
    }

    public async Task<bool> Disable(int id)
    {
        logger.LogDebug("Disable()");

        var recipe = await recipeRepository.GetById(id, userContext.GetUserId());

        if (recipe == null)
            return false;

        recipe.Disable();
        await recipeRepository.Update(recipe);

        return true;
    }
    private static RecipeResponse ToResponse(Recipe recipe)
    {
        return new RecipeResponse(recipe);
    }
}
