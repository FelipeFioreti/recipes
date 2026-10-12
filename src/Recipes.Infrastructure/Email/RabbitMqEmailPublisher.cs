using System.Text.Json;
using RabbitMQ.Client;
using Recipes.Application.Interfaces.Email;
using Recipes.Infrastructure.Messaging;
using Recipes.Messaging.Email;

namespace Recipes.Infrastructure.Email;

public class RabbitMqEmailPublisher(
    RabbitMqConnectionProvider connectionProvider,
    ILogger<RabbitMqEmailPublisher> logger) : IEmailService
{
    public async Task Send(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Send()");

        var message = new SendEmailMessage(Guid.NewGuid().ToString(), to, subject, htmlBody);
        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var properties = new BasicProperties
        {
            MessageId = message.MessageId,
            ContentType = "application/json",
            Persistent = true
        };

        var connection = await connectionProvider.GetConnection(cancellationToken);

        await using var channel = await connection.CreateChannelAsync(
            new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true),
            cancellationToken);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: EmailTopology.SendQueue,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        logger.LogInformation("Email message {MessageId} queued.", message.MessageId);
    }
}
