using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Recipes.Api.Domain.Exceptions;
using Recipes.Api.Presentation.Models;

namespace Recipes.Api.Presentation.Middlewares;

public class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            logger.LogWarning(exception, "The response has already started, rethrowing exception.");
            ExceptionDispatchInfo.Capture(exception).Throw();
        }

        var statusCode = GetStatusCode(exception);
        var message = GetMessage(exception, statusCode);

        if (statusCode >= (int)HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);
        else
            logger.LogWarning(exception, "Handled exception. TraceId: {TraceId}", context.TraceIdentifier);

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message,
            Details = environment.IsDevelopment() ? exception.ToString() : null,
            TraceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            AppException appException => appException.StatusCode,
            ValidationException => (int)HttpStatusCode.BadRequest,
            BadHttpRequestException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            FormatException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException or SecurityTokenException => (int)HttpStatusCode.Unauthorized,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    private string GetMessage(Exception exception, int statusCode)
    {
        if (statusCode >= (int)HttpStatusCode.InternalServerError && !environment.IsDevelopment())
            return "An unexpected error occurred.";

        return exception.Message;
    }
}
