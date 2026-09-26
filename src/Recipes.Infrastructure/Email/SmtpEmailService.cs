using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Recipes.Application.Interfaces.Email;

namespace Recipes.Infrastructure.Email;

public class SmtpEmailService(IOptions<SmtpSettings> smtpSettings, ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    public async Task Send(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Send()");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, _smtpSettings.Security, cancellationToken);

        if (!string.IsNullOrEmpty(_smtpSettings.Username))
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password, cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
