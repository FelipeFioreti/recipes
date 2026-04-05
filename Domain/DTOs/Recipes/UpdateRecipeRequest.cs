using System.ComponentModel.DataAnnotations;

namespace Recipes.Domain.DTOs.Recipes;

public record UpdateRecipeRequest
{
    [Required] public int Id { get; init; }

    [Required] [MaxLength(255)] public string Name { get; init; } = string.Empty;

    [MaxLength(2000)] public string Description { get; init; } = string.Empty;

    [Required] public int RecipeTypeId { get; init; }
}