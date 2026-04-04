namespace Recipes.Domain.DTOs.Auth;

public record RegisterUserRequest
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}