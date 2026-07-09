using System.ComponentModel.DataAnnotations;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record CreateStepRequest
{
    [Required]
    public int RecipeId { get; init; }

    [Required]
    public int Position { get; init; }

    [Required]
    [MaxLength(2000)]
    public string Description { get; init; } = string.Empty;
}
