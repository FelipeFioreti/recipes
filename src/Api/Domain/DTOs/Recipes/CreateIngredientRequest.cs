using System.ComponentModel.DataAnnotations;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record CreateIngredientRequest
{
    [Required]
    public int RecipeId { get; init; }

    [Required]
    [MaxLength(255)]
    public string Name { get; init; } = string.Empty;

    [Required]
    public decimal Quantity { get; init; }

    [Required]
    public int UnitId { get; init; }
}
