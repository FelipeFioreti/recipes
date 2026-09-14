using Recipes.Application.DTOs.Recipes;
using Recipes.Domain.Entities.Recipes;

namespace Recipes.Application.Mappings;

public static class IngredientMappings
{
    public static IngredientResponse ToResponse(this Ingredient ingredient)
    {
        return new IngredientResponse(ingredient);
    }
}
