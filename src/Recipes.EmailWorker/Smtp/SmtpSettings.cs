using MailKit.Security;

namespace Recipes.EmailWorker.Smtp;

public class SmtpSettings
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
    public SecureSocketOptions Security { get; init; } = SecureSocketOptions.Auto;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FromAddress { get; init; } = string.Empty;
    public string FromName { get; init; } = "ChefArchive";
}
