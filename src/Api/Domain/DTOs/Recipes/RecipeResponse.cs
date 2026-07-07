using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record RecipeResponse
{
    public RecipeResponse(Recipe recipe)
    {
        Id = recipe.Id;
        Name = recipe.Name;
        Description = recipe.Description;
        CategoryId = recipe.CategoryId;
        UserId = recipe.UserId;

        Category = recipe.Category is null
            ? null
            : new CategoryResponse(recipe.Category);

        CreatedAt = recipe.CreatedAt;
        UpdatedAt = recipe.UpdatedAt;
        DeletedAt = recipe.DeletedAt;
    }

    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int CategoryId { get; init; }
    public int UserId { get; init; }

    public CategoryResponse? Category { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}
