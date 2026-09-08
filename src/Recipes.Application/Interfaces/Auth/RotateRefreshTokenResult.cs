using Recipes.Domain.Entities.Users;

namespace Recipes.Application.Interfaces.Auth;

public record RotateRefreshTokenResult(User User, string Token);
