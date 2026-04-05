namespace Recipes.Domain.DTOs.Recipes;

public record CreateRecipeTypeRequest
{
    public required string Name { get; set; }
}