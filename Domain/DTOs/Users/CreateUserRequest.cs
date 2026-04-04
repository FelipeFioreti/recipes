using System.ComponentModel.DataAnnotations;

namespace Recipes.Domain.DTOs.Users;

public record CreateUserRequest
{
    [Required]
    [MaxLength(255)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(255)]
    public string Password { get; init; } = string.Empty;
}
