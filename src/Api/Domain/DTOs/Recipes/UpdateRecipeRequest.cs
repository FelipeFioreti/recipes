using System.ComponentModel.DataAnnotations;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record UpdateRecipeRequest
{
    public int Id { get; init; }
    [Required]
    [MaxLength(255)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(2000)] public string Description { get; init; } = string.Empty;

    [Required] public int CategoryId { get; init; }

    public IReadOnlyCollection<RecipeIngredientRequest> Ingredients { get; init; } = [];
    public IReadOnlyCollection<RecipeStepRequest> Steps { get; init; } = [];
}
