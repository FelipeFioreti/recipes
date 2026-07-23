using System.ComponentModel.DataAnnotations;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record RecipeIngredientRequest
{
    public int? Id { get; init; }

    [Required]
    [MaxLength(255)]
    public string Name { get; init; } = string.Empty;

    [Range(
        typeof(decimal),
        "0.01",
        "9999999999999999.99",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true)]
    public decimal Quantity { get; init; }

    [Range(1, int.MaxValue)]
    public int UnitId { get; init; }
}
