using System.ComponentModel.DataAnnotations;

namespace Recipes.Application.DTOs.Auth;

public record ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string Email { get; set; }
}
