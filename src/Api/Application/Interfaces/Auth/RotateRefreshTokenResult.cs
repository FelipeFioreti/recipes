using Recipes.Api.Domain.Entities.Users;

namespace Recipes.Api.Application.Interfaces.Auth;

public record RotateRefreshTokenResult(User User, string Token);
