using Recipes.Api.Domain.DTOs.Auth;
using Recipes.Api.Domain.DTOs.Users;

namespace Recipes.Api.Domain.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthenticateResponse?> Authenticate(AuthenticateRequest authenticateRequest);
    Task<UserResponse?> Register(RegisterUserRequest registerUserRequest);
}