using System.ComponentModel.DataAnnotations;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Application.DTOs.Recipes;

public record RecipeStepRequest : IRecipeStepData
{
    public int? Id { get; init; }

    [Range(1, int.MaxValue)]
    public int Position { get; init; }

    [Required]
    [MaxLength(2000)]
    public string Description { get; init; } = string.Empty;
}
