using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Recipes.Domain.Interfaces.Auth;

namespace Recipes.Infrastructure.Security;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public int? UserId =>
        TryGetIntClaim(ClaimTypes.NameIdentifier) ??
        TryGetIntClaim(JwtRegisteredClaimNames.Sub) ??
        TryGetIntClaim("id");

    private int? TryGetIntClaim(string claimType)
    {
        var value = Principal?.FindFirstValue(claimType);

        return int.TryParse(value, out var userId) ? userId : null;
    }
}