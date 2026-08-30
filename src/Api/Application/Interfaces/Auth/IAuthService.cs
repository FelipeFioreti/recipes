using Recipes.Api.Domain.DTOs.Auth;
using Recipes.Api.Domain.DTOs.Users;

namespace Recipes.Api.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthenticateResponse?> Login(AuthenticateRequest request);
    Task<UserResponse?> Register(RegisterUserRequest registerUserRequest);
    Task<bool> Logout(LogoutRequest request);
    Task<AuthenticateResponse?> RefreshToken(RefreshTokenRequest request);
}
