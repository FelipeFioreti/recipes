using System.Net;

namespace Recipes.Api.Domain.Exceptions;

public class NotFoundException(string message) : AppException(message)
{
    public override int StatusCode => (int)HttpStatusCode.NotFound;
}
