using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Recipes.Api.Domain.Exceptions;
using Recipes.Api.Domain.Interfaces.Auth;

namespace Recipes.Api.Infrastructure.Security;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public int? UserId =>
        TryGetIntClaim(ClaimTypes.NameIdentifier, JwtRegisteredClaimNames.Sub, "id");

    public int GetUserId()
    {
        return UserId ?? throw new UnauthorizedException("Authenticated user id claim is missing.");
    }

    private int? TryGetIntClaim(params string[] claimTypes)
    {
        var value = Principal.FindFirstValueByTypes(claimTypes);

        return int.TryParse(value, out var userId) ? userId : null;
    }
}
