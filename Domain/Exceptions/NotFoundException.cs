using System.Net;

namespace Recipes.Domain.Exceptions;

public class NotFoundException(string message) : AppException(message)
{
    public override int StatusCode => (int)HttpStatusCode.NotFound;
}
