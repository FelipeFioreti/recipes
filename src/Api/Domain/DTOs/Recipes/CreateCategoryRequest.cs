namespace Recipes.Api.Domain.DTOs.Recipes;

public record CreateCategoryRequest
{
    public required string Name { get; set; }
}
