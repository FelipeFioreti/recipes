using System.Net;

namespace Recipes.Domain.Exceptions;

public class UnauthorizedException(string message = "Unauthorized.") : AppException(message)
{
    public override int StatusCode => (int)HttpStatusCode.Unauthorized;
}
