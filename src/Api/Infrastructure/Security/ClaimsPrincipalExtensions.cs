using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Recipes.Api.Infrastructure.Security;

public static class ClaimsPrincipalExtensions
{
    public static bool HasUserIdentifier(this ClaimsPrincipal user)
    {
        return user.HasAnyClaim([
            ClaimTypes.NameIdentifier,
            JwtRegisteredClaimNames.Sub
        ]);
    }

    public static bool HasUserEmail(this ClaimsPrincipal user)
    {
        return user.HasAnyClaim([
            ClaimTypes.Email,
            JwtRegisteredClaimNames.Email
        ]);
    }

    public static bool HasApplicationRole(this ClaimsPrincipal user, string role)
    {
        return user.Claims.Any(claim =>
            string.Equals(claim.Type, ClaimTypes.Role, StringComparison.Ordinal) &&
            string.Equals(claim.Value, role, StringComparison.OrdinalIgnoreCase));
    }

    public static string? FindFirstValueByTypes(this ClaimsPrincipal? user, params string[] claimTypes)
    {
        return claimTypes
            .Select(claimType => user?.FindFirst(claimType)?.Value)
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }

    private static bool HasAnyClaim(this ClaimsPrincipal user, IEnumerable<string> claimTypes)
    {
        return user.Claims.Any(claim => claimTypes.Contains(claim.Type));
    }
}
