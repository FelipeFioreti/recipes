using System.ComponentModel.DataAnnotations;

namespace Recipes.Application.DTOs.Auth;

public record LogoutRequest
{
    [Required] public required string RefreshToken { get; set; }
}
