using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Recipes.Api.Infrastructure.Security;

public static class ClaimsPrincipalExtensions
{
    private static readonly string[] UserIdClaimTypes =
    [
        ClaimTypes.NameIdentifier,
        JwtRegisteredClaimNames.Sub,
        "nameid",
        "id"
    ];

    private static readonly string[] EmailClaimTypes =
    [
        ClaimTypes.Email,
        JwtRegisteredClaimNames.Email,
        "email"
    ];

    private static readonly string[] RoleClaimTypes =
    [
        ClaimTypes.Role,
        "role"
    ];

    public static bool HasUserIdentifier(this ClaimsPrincipal user)
    {
        return user.HasAnyClaim(UserIdClaimTypes);
    }

    public static bool HasUserEmail(this ClaimsPrincipal user)
    {
        return user.HasAnyClaim(EmailClaimTypes);
    }

    public static bool HasApplicationRole(this ClaimsPrincipal user, string role)
    {
        return user.Claims.Any(claim =>
            RoleClaimTypes.Contains(claim.Type) &&
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
