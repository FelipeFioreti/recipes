using Recipes.Api.Domain.DTOs.Recipes;
using Recipes.Api.Domain.Entities.Recipes;
using Recipes.Api.Domain.Interfaces.Auth;
using Recipes.Api.Domain.Interfaces.Recipes;

namespace Recipes.Api.Application.Services.Recipes;

public class IngredientService(
    IIngredientRepository ingredientRepository,
    IRecipeRepository recipeRepository,
    IUserContext userContext,
    ILogger<IngredientService> logger) : IIngredientService
{
    public async Task<IEnumerable<IngredientResponse>> GetAll(int page, int size)
    {
        logger.LogDebug("GetAll()");

        var ingredients = userContext.IsAdmin()
            ? await ingredientRepository.GetAll(page, size)
            : await ingredientRepository.GetAllForUser(userContext.GetUserId(), page, size);

        return ingredients.Select(ToResponse);
    }

    public async Task<IngredientResponse?> GetById(int id)
    {
        logger.LogDebug("GetById()");

        var ingredient = userContext.IsAdmin()
            ? await ingredientRepository.GetById(id)
            : await ingredientRepository.GetByIdForUser(id, userContext.GetUserId());

        return ingredient == null ? null : ToResponse(ingredient);
    }

    public async Task<IngredientResponse?> Create(CreateIngredientRequest request)
    {
        logger.LogDebug("Create()");

        var hasAccessToRecipe = await recipeRepository.CanAccessRecipe(request.RecipeId, userContext.GetUserId());

        if (!hasAccessToRecipe)
            return null;

        var ingredient = await ingredientRepository.Create(
            new Ingredient(request.Name, request.Quantity, request.RecipeId, request.UnitId));

        return ingredient == null ? null : ToResponse(ingredient);
    }

    public async Task<IngredientResponse?> Update(int id, UpdateIngredientRequest request)
    {
        logger.LogDebug("Update()");

        var hasAccessToRecipe = await recipeRepository.CanAccessRecipe(request.RecipeId, userContext.GetUserId());

        if (!hasAccessToRecipe)
            return null;

        var existingIngredient = userContext.IsAdmin()
            ? await ingredientRepository.GetById(id)
            : await ingredientRepository.GetByIdForUser(id, userContext.GetUserId());

        if (existingIngredient == null)
            return null;

        existingIngredient.Update(request);

        var updatedIngredient = await ingredientRepository.Update(existingIngredient);

        return updatedIngredient == null
            ? null
            : ToResponse(updatedIngredient);
    }

    public async Task<bool> Disable(int id)
    {
        logger.LogDebug("Disable()");

        var ingredient = userContext.IsAdmin()
            ? await ingredientRepository.GetById(id)
            : await ingredientRepository.GetByIdForUser(id, userContext.GetUserId());

        if (ingredient == null)
            return false;

        ingredient.Disable();
        await ingredientRepository.Update(ingredient);

        return true;
    }
    
    private static IngredientResponse ToResponse(Ingredient ingredient)
    {
        return new IngredientResponse(ingredient);
    }
}