using Recipes.Api.Domain.Entities.Users;

namespace Recipes.Api.Domain.DTOs.Auth;

public record AuthenticateResponse
{
    public AuthenticateResponse(User user, string accessToken, string refreshToken)
    {
        Id = user.Id;
        Name = user.Name;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
