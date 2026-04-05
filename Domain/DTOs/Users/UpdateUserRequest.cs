using System.ComponentModel.DataAnnotations;

namespace Recipes.Domain.DTOs.Users;

public record UpdateUserRequest
{
    [Required] [MaxLength(255)] public string Name { get; init; } = string.Empty;

    [MinLength(6)] [MaxLength(255)] public string Password { get; init; } = string.Empty;
}