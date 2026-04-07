using System.Net;

namespace Recipes.Api.Domain.Exceptions;

public class ForbiddenException(string message = "Forbidden.") : AppException(message)
{
    public override int StatusCode => (int)HttpStatusCode.Forbidden;
}
