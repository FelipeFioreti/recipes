namespace Recipes.Application.DTOs.Recipes;

public record CreateCategoryRequest
{
    public required string Name { get; set; }
}
