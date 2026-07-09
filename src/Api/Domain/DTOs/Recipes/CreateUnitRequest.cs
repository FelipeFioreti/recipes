using System.ComponentModel.DataAnnotations;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record CreateUnitRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(10)]
    public string Abbreviation { get; init; } = string.Empty;
}
