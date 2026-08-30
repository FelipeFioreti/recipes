using System.ComponentModel.DataAnnotations;

namespace Recipes.Api.Domain.DTOs.Auth;

public record LogoutRequest
{
    [Required] public required string RefreshToken { get; set; }
}
