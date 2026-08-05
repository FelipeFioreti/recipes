using System.ComponentModel.DataAnnotations;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record UpdateUnitRequest
{
    [Key] public int Id { get; init; }

    [Required] [MaxLength(50)] public string Name { get; init; } = string.Empty;

    [Required] public bool ShowAbbreviation { get; init; }

    [MaxLength(10)] public string Abbreviation { get; init; } = string.Empty;
}