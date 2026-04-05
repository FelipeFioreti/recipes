using Recipes.Domain.Entities.Recipes;

namespace Recipes.Domain.DTOs.Recipes;

public record RecipeResponse
{
    public RecipeResponse(Recipe recipeType)
    {
        Id = recipeType.Id;
        Name = recipeType.Name;
        Description = recipeType.Description;
        RecipeTypeId = recipeType.RecipeTypeId;
        UserId = recipeType.UserId;
        CreatedAt = recipeType.CreatedAt;
        UpdatedAt = recipeType.UpdatedAt;
        DeletedAt = recipeType.DeletedAt;
    }

    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int RecipeTypeId { get; init; }
    public int UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}