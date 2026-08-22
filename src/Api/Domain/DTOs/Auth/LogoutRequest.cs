using System.ComponentModel.DataAnnotations;

namespace Recipes.Api.Domain.DTOs.Auth;

public record LogoutRequest
{
    [Required] public required int UserId { get; set; }
}