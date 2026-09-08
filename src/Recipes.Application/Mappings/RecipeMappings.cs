using Recipes.Application.DTOs.Recipes;
using Recipes.Domain.Entities.Recipes;

namespace Recipes.Application.Mappings;

public static class RecipeMappings
{
    public static RecipeResponse ToResponse(this Recipe recipe)
    {
        return new RecipeResponse(recipe);
    }

    public static Recipe ToEntity(this CreateRecipeRequest request, int userId)
    {
        return new Recipe(
            request.Name,
            request.Description,
            request.CategoryId,
            userId,
            request.Ingredients,
            request.Steps);
    }

    public static void ApplyTo(this UpdateRecipeRequest request, Recipe recipe)
    {
        recipe.Update(
            request.Name,
            request.Description,
            request.CategoryId,
            request.Ingredients,
            request.Steps);
    }
}
