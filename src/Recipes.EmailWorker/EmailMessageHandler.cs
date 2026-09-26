using System.Text.Json;
using Recipes.EmailWorker.Smtp;
using Recipes.Messaging.Email;

namespace Recipes.EmailWorker;

public enum EmailHandlingResult
{
    Ack,
    DeadLetter
}

public class EmailRetrySettings
{
    public IReadOnlyList<TimeSpan> Delays { get; init; } =
        [TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30)];
}

public class EmailMessageHandler(
    IEmailSender emailSender,
    EmailRetrySettings retrySettings,
    ILogger<EmailMessageHandler> logger)
{
    // O corpo nunca e logado: o email de recuperacao carrega o link com o token de reset.
    public async Task<EmailHandlingResult> Handle(ReadOnlyMemory<byte> body, CancellationToken cancellationToken)
    {
        var message = Deserialize(body);

        if (message == null)
            return EmailHandlingResult.DeadLetter;

        for (var attempt = 1;; attempt++)
        {
            try
            {
                await emailSender.Send(message.To, message.Subject, message.HtmlBody, cancellationToken);

                logger.LogInformation("Email message {MessageId} sent to {Recipient}.",
                    message.MessageId, MaskEmail(message.To));

                return EmailHandlingResult.Ack;
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
            {
                if (attempt > retrySettings.Delays.Count)
                {
                    logger.LogError(exception, "Giving up on email message {MessageId} after {Attempts} attempts.",
                        message.MessageId, attempt);

                    return EmailHandlingResult.DeadLetter;
                }

                logger.LogWarning(exception, "Attempt {Attempt} to send email message {MessageId} failed.",
                    attempt, message.MessageId);

                await Task.Delay(retrySettings.Delays[attempt - 1], cancellationToken);
            }
        }
    }

    private SendEmailMessage? Deserialize(ReadOnlyMemory<byte> body)
    {
        try
        {
            var message = JsonSerializer.Deserialize<SendEmailMessage>(body.Span);

            if (message is { To.Length: > 0, Subject.Length: > 0, HtmlBody: not null })
                return message;

            logger.LogError("Discarding email message {MessageId} with missing fields.", message?.MessageId);
        }
        catch (JsonException exception)
        {
            logger.LogError(exception, "Discarding malformed email message.");
        }

        return null;
    }

    public static string MaskEmail(string email)
    {
        var at = email.IndexOf('@');

        return at <= 0 ? "***" : $"{email[0]}***{email[at..]}";
    }
}
