using System.ComponentModel.DataAnnotations;

namespace Recipes.Application.DTOs.Users;

public record UpdateUserRequest
{
    [Key] public int Id { get; set; }
    [Required] [MaxLength(255)] public string Name { get; init; } = string.Empty;
}
