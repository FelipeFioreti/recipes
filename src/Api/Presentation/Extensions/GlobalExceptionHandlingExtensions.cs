using Recipes.Api.Presentation.Middlewares;

namespace Recipes.Api.Presentation.Extensions;

public static class GlobalExceptionHandlingExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}