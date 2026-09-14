using System.ComponentModel.DataAnnotations;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Application.DTOs.Recipes;

public record RecipeIngredientRequest : IRecipeIngredientData
{
    public int? Id { get; init; }

    [Required] [MaxLength(255)] public string Name { get; init; } = string.Empty;

    [Range(1, int.MaxValue)] public decimal Quantity { get; init; }

    [Range(1, int.MaxValue)] public int UnitId { get; init; }
}
