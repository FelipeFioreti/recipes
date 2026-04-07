using System.ComponentModel.DataAnnotations;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record UpdateRecipeTypeRequest
{
    [Required]
    [MaxLength(255)]
    public string Name { get; init; } = string.Empty;
}
