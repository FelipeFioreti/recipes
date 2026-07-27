using System.ComponentModel.DataAnnotations;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record UpdateStepRequest
{
    public int Id { get; init; }

    [Required] public int Position { get; init; }

    [Required] [MaxLength(2000)] public string Description { get; init; } = string.Empty;
}