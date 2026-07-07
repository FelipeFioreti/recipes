using System.ComponentModel.DataAnnotations;

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
}
