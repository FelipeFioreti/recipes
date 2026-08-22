using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Recipes.Api.Domain.DTOs.Auth;
using Recipes.Api.Domain.DTOs.Users;
using Recipes.Api.Domain.Entities.Enums;
using Recipes.Api.Domain.Entities.Settings;
using Recipes.Api.Domain.Entities.Token;
using Recipes.Api.Domain.Entities.Users;
using Recipes.Api.Domain.Interfaces.Auth;
using Recipes.Api.Domain.Interfaces.Token;
using Recipes.Api.Domain.Interfaces.Users;

namespace Recipes.Api.Application.Services.Auth;

public class AuthService(
    IUserRepository userRepository,
    IUserService userService,
    IPasswordService passwordService,
    IRefreshTokenRepository refreshTokenRepository,
    IOptions<AppSettings> appSettings,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model)
    {
        logger.LogDebug("Authenticate()");

        var user = await VerifyUser(model);

        if (user == null) return null;

        var token = await refreshTokenRepository.Create(new RefreshToken("", new DateTime().AddDays(30), user));

        return token == null ? null : new AuthenticateResponse(user, GenerateJwtToken(user));
    }

    public async Task<UserResponse?> Register(RegisterUserRequest registerUserRequest)
    {
        logger.LogDebug("RegisterUser()");

        return await userService.Create(new CreateUserRequest(
            registerUserRequest.Name,
            registerUserRequest.Email,
            registerUserRequest.Password));
    }

    public async Task<bool> Logout(LogoutRequest registerUserRequest)
    {
        logger.LogDebug("Logout()");

        var user = await userService.GetById(registerUserRequest.UserId);

        if (user == null) return false;

        var token = await refreshTokenRepository.GetTokenByUserId(user.Id);

        if (token == null) return false;

        await refreshTokenRepository.Delete(token);

        return true;
    }

    public async Task<AuthenticateResponse?> RefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        logger.LogDebug("RefreshToken()");

        var token = await refreshTokenRepository.GetTokenByUserId(refreshTokenRequest.UserId);

        if (token != null && token.IsExpired()) return null;

        var user = await userRepository.GetById(refreshTokenRequest.UserId);

        return user == null ? null : new AuthenticateResponse(user, GenerateJwtToken(user));
    }

    private async Task<User?> VerifyUser(AuthenticateRequest model)
    {
        logger.LogDebug("VerifyUser()");

        var user = await userRepository.GetByEmail(model.Email);

        if (user == null)
            return null;

        return passwordService.VerifyPassword(model.Password, user.PasswordHash)
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
            Expires = now.AddMinutes(_appSettings.TokenExpirationMinutes),
            NotBefore = now,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}