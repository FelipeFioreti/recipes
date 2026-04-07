using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record RecipeTypeResponse
{
    public RecipeTypeResponse(RecipeType recipeType)
    {
        Id = recipeType.Id;
        Name = recipeType.Name;

        CreatedAt = recipeType.CreatedAt;
        UpdatedAt = recipeType.UpdatedAt;
        DeletedAt = recipeType.DeletedAt;
    }

    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}