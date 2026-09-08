using Recipes.Application.DTOs.Recipes;
using Recipes.Domain.Entities.Recipes;

namespace Recipes.Application.Mappings;

public static class CategoryMappings
{
    public static CategoryResponse ToResponse(this Category category)
    {
        return new CategoryResponse(category);
    }
}
