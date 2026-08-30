using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Recipes.Api.Application.Interfaces.Auth;
using Recipes.Api.Domain.DTOs.Auth;
using Recipes.Api.Domain.DTOs.Users;
using Recipes.Api.Domain.Entities.Enums;
using Recipes.Api.Domain.Entities.Settings;
using Recipes.Api.Domain.Entities.Users;
using Recipes.Api.Domain.Interfaces.Auth;
using Recipes.Api.Domain.Interfaces.Users;

namespace Recipes.Api.Application.Services.Auth;

public class AuthService(
    IUserRepository userRepository,
    IUserService userService,
    IPasswordService passwordService,
    IRefreshTokenService refreshTokenService,
    IOptions<AppSettings> appSettings,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<AuthenticateResponse?> Login(AuthenticateRequest request)
    {
        logger.LogDebug("Login()");

        var user = await VerifyUser(request);

        if (user == null) return null;

        var refreshToken = await refreshTokenService.Create(user);

        return refreshToken == null ? null : new AuthenticateResponse(user, GenerateJwtToken(user), refreshToken);
    }

    public async Task<UserResponse?> Register(RegisterUserRequest registerUserRequest)
    {
        logger.LogDebug("RegisterUser()");

        return await userService.Create(new CreateUserRequest(
            registerUserRequest.Name,
            registerUserRequest.Email,
            registerUserRequest.Password));
    }

    public async Task<bool> Logout(LogoutRequest request)
    {
        logger.LogDebug("Logout()");

        var refreshToken = await refreshTokenService.GetByHash(request.RefreshToken);

        if (refreshToken == null) return false;
        
        refreshToken.Revoke();
        await refreshTokenService.Update(refreshToken);

        return true;
    }

    public async Task<AuthenticateResponse?> RefreshToken(RefreshTokenRequest request)
    {
        logger.LogDebug("RefreshToken()");

        var refreshToken = await refreshTokenService.Rotate(request.RefreshToken);

        return refreshToken == null
            ? null
            : new AuthenticateResponse(
                refreshToken.User,
                GenerateJwtToken(refreshToken.User),
                refreshToken.Token);
    }

    private async Task<User?> VerifyUser(AuthenticateRequest request)
    {
        logger.LogDebug("VerifyUser()");

        var user = await userRepository.GetByEmail(request.Email);

        if (user == null)
            return null;

        return passwordService.VerifyPassword(request.Password, user.PasswordHash)
            ? user
            : null;
    }

    private string GenerateJwtToken(User user)
    {
        logger.LogDebug("GenerateJwtToken()");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.Name, user.Name),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user.Role == Roles.ADMIN) claims.Add(new Claim(ClaimTypes.Role, nameof(Roles.USER)));

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_appSettings.Secret);
        var now = DateTime.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _appSettings.Issuer,
            Audience = _appSettings.Audience,
            Expires = now.AddMinutes(_appSettings.AccessTokenExpirationMinutes),
            NotBefore = now,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
