namespace Recipes.Domain.DTOs.Auth;

public record AuthenticateRequest
{
    public AuthenticateRequest(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public required string Email { get; set; }
    public required string Password { get; set; }
}