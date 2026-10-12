using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Recipes.Application.Interfaces.Auth;
using Recipes.Application.Interfaces.Email;
using Recipes.Domain.Interfaces.Recipes;
using Recipes.Domain.Interfaces.Token;
using Recipes.Domain.Interfaces.Users;
using Recipes.Infrastructure.Data.Context;
using Recipes.Infrastructure.Email;
using Recipes.Infrastructure.Jobs;
using Recipes.Infrastructure.Messaging;
using Recipes.Infrastructure.Repositories;
using Recipes.Infrastructure.Security;
using Recipes.Messaging;

namespace Recipes.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IPasswordService, PasswordService>();

        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
        services.AddSingleton<RabbitMqConnectionProvider>();
        services.AddScoped<IEmailService, RabbitMqEmailPublisher>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IStepRepository, StepRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();

        services.AddQuartz(options =>
        {
            var refreshTokenCleanupJobKey = new JobKey(nameof(RefreshTokenCleanupJob));

            options.AddJob<RefreshTokenCleanupJob>(job => job.WithIdentity(refreshTokenCleanupJobKey));
            options.AddTrigger(trigger => trigger
                .ForJob(refreshTokenCleanupJobKey)
                .WithIdentity("RefreshTokenCleanupTrigger")
                .StartNow()
                .WithCronSchedule("0 0 3 ? * *", schedule => schedule.InTimeZone(TimeZoneInfo.Local)));

            var passwordResetTokenCleanupJobKey = new JobKey(nameof(PasswordResetTokenCleanupJob));

            options.AddJob<PasswordResetTokenCleanupJob>(job => job.WithIdentity(passwordResetTokenCleanupJobKey));
            options.AddTrigger(trigger => trigger
                .ForJob(passwordResetTokenCleanupJobKey)
                .WithIdentity("PasswordResetTokenCleanupTrigger")
                .StartNow()
                .WithCronSchedule("0 0 3 ? * *", schedule => schedule.InTimeZone(TimeZoneInfo.Local)));
        });
        services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });

        return services;
    }
}
