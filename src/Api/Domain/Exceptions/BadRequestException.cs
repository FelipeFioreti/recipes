using System.Net;

namespace Recipes.Api.Domain.Exceptions;

public class BadRequestException(string message) : AppException(message)
{
    public override int StatusCode => (int)HttpStatusCode.BadRequest;
}
