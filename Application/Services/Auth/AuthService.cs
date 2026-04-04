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

public class AuthService(IUserService service, IOptions<AppSettings> appSettings, ILogger<AuthService> logger)
    : IAuthService
{
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model)
    {
        logger.LogDebug("Authenticate()");

        var user = await VerifyUser(model);

        if (user == null)
            return null;

        var token = GenerateJwtToken(user);

        return new AuthenticateResponse(user, token);
    }

    public async Task<UserResponse?> RegisterUser(RegisterUserRequest registerUserRequest)
    {
        logger.LogDebug("RegisterUser()");

        var user = new User
        {
            Name = registerUserRequest.Name,
            Email = registerUserRequest.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(registerUserRequest.Password),
            Role = registerUserRequest.Role
        };

        var createdUser = await service.Create(user);

        return createdUser == null ? null : new UserResponse(createdUser);
    }

    private async Task<User?> VerifyUser(AuthenticateRequest model)
    {
        logger.LogDebug("VerifyUser()");

        var user = await service.GetByEmail(model.Email);

        if (user == null)
            return null;

        return BCrypt.Net.BCrypt.Verify(model.Password, user.Password)
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
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}