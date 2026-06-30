using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record RecipeResponse
{
    public RecipeResponse(Recipe recipe)
    {
        Id = recipe.Id;
        Name = recipe.Name;
        Description = recipe.Description;
        RecipeTypeId = recipe.RecipeTypeId;
        UserId = recipe.UserId;

        RecipeType = recipe.RecipeType is null
            ? null
            : new RecipeTypeResponse(recipe.RecipeType);

        CreatedAt = recipe.CreatedAt;
        UpdatedAt = recipe.UpdatedAt;
        DeletedAt = recipe.DeletedAt;
    }

    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int RecipeTypeId { get; init; }
    public int UserId { get; init; }

    public RecipeTypeResponse? RecipeType { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}