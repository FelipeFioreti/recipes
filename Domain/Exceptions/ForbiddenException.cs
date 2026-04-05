using System.Net;

namespace Recipes.Domain.Exceptions;

public class ForbiddenException(string message = "Forbidden.") : AppException(message)
{
    public override int StatusCode => (int)HttpStatusCode.Forbidden;
}
