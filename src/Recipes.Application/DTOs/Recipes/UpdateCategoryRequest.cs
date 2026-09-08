using System.ComponentModel.DataAnnotations;

namespace Recipes.Application.DTOs.Recipes;

public record UpdateCategoryRequest
{
    [Key]
    public int Id { get; init; }
    [Required]
    [MaxLength(255)]
    public string Name { get; init; } = string.Empty;
}
