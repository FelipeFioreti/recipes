using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record CategoryResponse
{
    public CategoryResponse(Category category)
    {
        Id = category.Id;
        Name = category.Name;

        CreatedAt = category.CreatedAt;
        UpdatedAt = category.UpdatedAt;
        DeletedAt = category.DeletedAt;
    }

    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}
