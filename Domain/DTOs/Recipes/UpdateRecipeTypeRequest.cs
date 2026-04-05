namespace Recipes.Domain.DTOs.Recipes;

public record UpdateRecipeTypeRequest
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}