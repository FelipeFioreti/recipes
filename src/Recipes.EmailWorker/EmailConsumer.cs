using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Recipes.Messaging;
using Recipes.Messaging.Email;

namespace Recipes.EmailWorker;

public class EmailConsumer(
    IOptions<RabbitMqSettings> rabbitMqSettings,
    EmailMessageHandler handler,
    ILogger<EmailConsumer> logger) : BackgroundService
{
    private const string ClientName = "chefarchive-email-worker";
    private static readonly TimeSpan ReconnectDelay = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var connection = await Connect(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await EmailTopology.DeclareAsync(channel, stoppingToken);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, delivery) =>
        {
            EmailHandlingResult result;
            try
            {
                result = await handler.Handle(delivery.Body, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Sem ack: o broker devolve a mensagem para a fila quando o canal fechar.
                return;
            }

            if (result == EmailHandlingResult.Ack)
                await channel.BasicAckAsync(delivery.DeliveryTag, multiple: false);
            else
                await channel.BasicNackAsync(delivery.DeliveryTag, multiple: false, requeue: false);
        };

        await channel.BasicConsumeAsync(EmailTopology.SendQueue, autoAck: false, consumer, stoppingToken);
        logger.LogInformation("Consuming queue {Queue}.", EmailTopology.SendQueue);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task<IConnection> Connect(CancellationToken stoppingToken)
    {
        var factory = rabbitMqSettings.Value.CreateConnectionFactory(ClientName);

        while (true)
        {
            try
            {
                return await factory.CreateConnectionAsync(stoppingToken);
            }
            catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning(exception, "RabbitMQ unavailable, retrying in {Delay}.", ReconnectDelay);
                await Task.Delay(ReconnectDelay, stoppingToken);
            }
        }
    }
}
