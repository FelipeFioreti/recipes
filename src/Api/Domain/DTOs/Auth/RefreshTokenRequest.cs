namespace Recipes.Api.Domain.DTOs.Auth;

public record RefreshTokenRequest(int UserId)
{
    public int UserId { get; set; } = UserId;
}