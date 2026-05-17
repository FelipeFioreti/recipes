using Recipes.Api.Domain.DTOs.Recipes;
using Recipes.Api.Domain.Entities.Recipes;
using Recipes.Api.Domain.Interfaces.Auth;
using Recipes.Api.Domain.Interfaces.Recipes;

namespace Recipes.Api.Application.Services.Recipes;

public class RecipeService(
    IRecipeRepository recipeRepository,
    IUserContext userContext,
    ILogger<RecipeService> logger) : IRecipeService
{
    public async Task<IEnumerable<RecipeResponse>> GetAll()
    {
        logger.LogDebug("GetAll()");

        var recipes = userContext.IsAdmin()
            ? await recipeRepository.GetAll()
            : await recipeRepository.GetAllForUser(userContext.GetUserId());

        return recipes.Select(ToResponse);
    }

    public async Task<RecipeResponse?> GetById(int id)
    {
        logger.LogDebug("GetById()");

        var recipe = userContext.IsAdmin()
            ? await recipeRepository.GetById(id)
            : await recipeRepository.GetByIdForUser(id, userContext.GetUserId());

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

        var existingRecipe = userContext.IsAdmin()
            ? await recipeRepository.GetById(id)
            : await recipeRepository.GetByIdForUser(id, userContext.GetUserId());

        if (existingRecipe == null)
            return null;

        existingRecipe.Update(request);

        var updatedRecipe = await recipeRepository.Update(existingRecipe);

        return updatedRecipe == null ? null : ToResponse(updatedRecipe);
    }

    public async Task<bool> Disable(int id)
    {
        logger.LogDebug("Disable()");

        var recipe = userContext.IsAdmin()
            ? await recipeRepository.GetById(id)
            : await recipeRepository.GetByIdForUser(id, userContext.GetUserId());

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