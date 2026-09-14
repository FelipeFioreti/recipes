using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Recipes.Application.Interfaces.Auth;
using Recipes.Application.Interfaces.Engine;
using Recipes.Application.Interfaces.Recipes;
using Recipes.Application.Interfaces.Users;
using Recipes.Application.Services.Auth;
using Recipes.Application.Services.Engine;
using Recipes.Application.Services.Recipes;
using Recipes.Application.Services.Users;
using Recipes.Application.Settings;

namespace Recipes.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IEngineService, EngineService>();
        services.AddScoped<IRefreshTokenCleanupRoutine, RefreshTokenCleanupRoutine>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IIngredientService, IngredientService>();
        services.AddScoped<IStepService, StepService>();
        services.AddScoped<IUnitService, UnitService>();

        return services;
    }
}
