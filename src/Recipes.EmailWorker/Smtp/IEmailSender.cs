namespace Recipes.EmailWorker.Smtp;

public interface IEmailSender
{
    Task Send(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
