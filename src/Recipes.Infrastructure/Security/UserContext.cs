using System.Security.Claims;
using Recipes.Application.Interfaces.Auth;
using Recipes.Domain.Entities.Enums;
using Recipes.Domain.Exceptions;

namespace Recipes.Infrastructure.Security;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private ClaimsPrincipal Principal =>
        httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedException("Authenticated user not found.");

    public int GetUserId()
    {
        IsAuthenticated();

        var claimValue = Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return int.TryParse(claimValue, out var userId)
            ? userId
            : throw new UnauthorizedException("Authenticated user id claim is missing.");
    }

    public bool IsAdmin()
    {
        IsAuthenticated();

        return Principal.IsInRole(nameof(Roles.ADMIN));
    }

    private void IsAuthenticated()
    {
        if (Principal.Identity?.IsAuthenticated != true)
            throw new UnauthorizedException(
                "User is not authenticated.");
    }
}
