using Recipes.Domain.DTOs.Auth;
using Recipes.Domain.DTOs.Users;

namespace Recipes.Domain.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthenticateResponse?> Authenticate(AuthenticateRequest authenticateRequest);
    Task<UserResponse?> Register(RegisterUserRequest registerUserRequest);
}