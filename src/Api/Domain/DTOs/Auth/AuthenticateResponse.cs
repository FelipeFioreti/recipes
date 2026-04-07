using Recipes.Api.Domain.Entities.Users;

namespace Recipes.Api.Domain.DTOs.Auth;

public record AuthenticateResponse
{
    public AuthenticateResponse(User user, string token)
    {
        Id = user.Id;
        Name = user.Name;
        Token = token;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Token { get; set; }
}