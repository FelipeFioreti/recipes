namespace Recipes.Domain.Entities.Settings;

public class AppSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int TokenExpirationDays { get; set; } = 7;
}
