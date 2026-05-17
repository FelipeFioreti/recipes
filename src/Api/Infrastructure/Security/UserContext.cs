using System.Security.Claims;
using Recipes.Api.Domain.Entities.Enums;
using Recipes.Api.Domain.Exceptions;
using Recipes.Api.Domain.Interfaces.Auth;

namespace Recipes.Api.Infrastructure.Security;

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