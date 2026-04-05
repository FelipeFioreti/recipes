using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Recipes.Domain.DTOs.Auth;
using Recipes.Domain.DTOs.Users;
using Recipes.Domain.Entities.Settings;
using Recipes.Domain.Entities.Users;
using Recipes.Domain.Interfaces.Auth;
using Recipes.Domain.Interfaces.Users;

namespace Recipes.Application.Services.Auth;

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

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_appSettings.Secret);
        var now = DateTime.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ]),
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