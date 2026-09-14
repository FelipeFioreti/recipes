using Recipes.Application.DTOs.Recipes;
using Recipes.Application.Interfaces.Auth;
using Recipes.Application.Interfaces.Recipes;
using Recipes.Application.Mappings;
using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Application.Services.Recipes;

public class RecipeService(
    IRecipeRepository recipeRepository,
    IUserContext userContext,
    ILogger<RecipeService> logger) : IRecipeService
{
    public async Task<IEnumerable<RecipeResponse>> GetAll(int page, int size)
    {
        logger.LogDebug("GetAll()");

        var recipes = userContext.IsAdmin()
            ? await recipeRepository.GetAll(page, size)
            : await recipeRepository.GetAllForUser(userContext.GetUserId(), page, size);

        return recipes.Select(recipe => recipe.ToResponse());
    }

    public async Task<RecipeResponse?> GetById(int id)
    {
        logger.LogDebug("GetById()");

        var recipe = userContext.IsAdmin()
            ? await recipeRepository.GetById(id)
            : await recipeRepository.GetByIdForUser(id, userContext.GetUserId());

        return recipe?.ToResponse();
    }

    public async Task<RecipeResponse?> Create(CreateRecipeRequest request)
    {
        logger.LogDebug("Create()");

        var recipe = request.ToEntity(userContext.GetUserId());

        var createdRecipe = await recipeRepository.Create(recipe);

        return createdRecipe?.ToResponse();
    }

    public async Task<RecipeResponse?> Update(UpdateRecipeRequest request)
    {
        logger.LogDebug("Update()");

        var existingRecipe = userContext.IsAdmin()
            ? await recipeRepository.GetById(request.Id, true)
            : await recipeRepository.GetByIdForUser(request.Id, userContext.GetUserId(), true);

        if (existingRecipe == null)
            return null;

        request.ApplyTo(existingRecipe);

        var updatedRecipe = await recipeRepository.Update(existingRecipe);

        return updatedRecipe?.ToResponse();
    }

    public async Task<bool> Disable(int id)
    {
        logger.LogDebug("Disable()");

        var recipe = userContext.IsAdmin()
            ? await recipeRepository.GetById(id, true)
            : await recipeRepository.GetByIdForUser(id, userContext.GetUserId(), true);

        if (recipe == null)
            return false;

        recipe.Disable();
        await recipeRepository.Update(recipe);

        return true;
    }
}
