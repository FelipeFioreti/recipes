namespace Recipes.Api.Domain.Entities.Settings;

public class AppSettings
{
    public string Secret { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int TokenExpirationMinutes { get; init; } = 10;
}