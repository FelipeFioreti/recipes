using System.ComponentModel.DataAnnotations;
using Recipes.Domain.Entities.Enums;

namespace Recipes.Domain.DTOs.Users;

public record CreateUserRequest
{
    [Required] [MaxLength(255)] public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(255)]
    public string Password { get; init; } = string.Empty;

    [Required] public Roles Role { get; init; } = Roles.USER;
}