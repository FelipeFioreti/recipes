using Recipes.Application.DTOs.Auth;
using Recipes.Application.DTOs.Users;

namespace Recipes.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthenticateResponse?> Login(AuthenticateRequest request);
    Task<UserResponse?> Register(RegisterUserRequest registerUserRequest);
    Task<bool> Logout(LogoutRequest request);
    Task<AuthenticateResponse?> RefreshToken(RefreshTokenRequest request);
}
