namespace Recipes.Api.Domain.DTOs.Auth;

public record AuthenticateRequest(string Email, string Password)
{
    public required string Email { get; set; } = Email;
    public required string Password { get; set; } = Password;
}