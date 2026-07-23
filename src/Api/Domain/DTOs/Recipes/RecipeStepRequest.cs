using System.ComponentModel.DataAnnotations;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record RecipeStepRequest
{
    public int? Id { get; init; }

    [Range(1, int.MaxValue)]
    public int Position { get; init; }

    [Required]
    [MaxLength(2000)]
    public string Description { get; init; } = string.Empty;
}
