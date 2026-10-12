namespace Recipes.Application.Interfaces.Email;

public interface IEmailService
{
    Task Send(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
