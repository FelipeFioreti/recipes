using System.ComponentModel.DataAnnotations;

namespace Recipes.Application.DTOs.Auth;

public record ResetPasswordRequest
{
    [Required] [MaxLength(255)] public required string Token { get; set; }

    [Required]
    [MinLength(6)]
    [MaxLength(255)]
    public required string NewPassword { get; set; }
}
