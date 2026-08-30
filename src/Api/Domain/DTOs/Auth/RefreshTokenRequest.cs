namespace Recipes.Api.Domain.DTOs.Auth;

public record RefreshTokenRequest(string RefreshToken)
{
    public required string RefreshToken { get; init; } = RefreshToken;
}
