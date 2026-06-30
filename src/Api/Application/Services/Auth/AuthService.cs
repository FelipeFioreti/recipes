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
using Recipes.Api.Domain.Entities.Users;
using Recipes.Api.Domain.Interfaces.Auth;
using Recipes.Api.Domain.Interfaces.Users;

namespace Recipes.Api.Application.Services.Auth;

public class AuthService(
    IUserRepository userRepository,
    IUserService service,
    IPasswordService passwordService,
    IOptions<AppSettings> appSettings,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model)
    {
        logger.LogDebug("Authenticate()");

        var user = await VerifyUser(model);

        return user == null ? null : new AuthenticateResponse(user, GenerateJwtToken(user));
    }

    public async Task<UserResponse?> Register(RegisterUserRequest registerUserRequest)
    {
        logger.LogDebug("RegisterUser()");

        return await service.Create(new CreateUserRequest(
            registerUserRequest.Name,
            registerUserRequest.Email,
            registerUserRequest.Password));
    }

    private async Task<User?> VerifyUser(AuthenticateRequest model)
    {
        logger.LogDebug("VerifyUser()");

        var user = await userRepository.GetByEmail(model.Email);

        if (user == null)
            return null;

        return passwordService.VerifyPassword(model.Password, user.Password)
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
            Expires = now.AddDays(_appSettings.TokenExpirationDays),
            NotBefore = now,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}