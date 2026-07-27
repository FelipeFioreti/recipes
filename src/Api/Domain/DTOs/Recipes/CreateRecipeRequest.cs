using System.ComponentModel.DataAnnotations;
using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record CreateRecipeRequest
{
    [Required]
    [MaxLength(255)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; init; } = string.Empty;

    [Required]
    public int CategoryId { get; init; }

    public ICollection<RecipeIngredientRequest> Ingredients { get; init; } = [];
    public ICollection<RecipeStepRequest> Steps { get; init; } = [];
}
