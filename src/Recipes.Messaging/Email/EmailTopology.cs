using RabbitMQ.Client;

namespace Recipes.Messaging.Email;

public static class EmailTopology
{
    public const string SendQueue = "email.send";
    public const string DeadLetterQueue = "email.send.dead";

    private static readonly int DeadLetterTtlMilliseconds = (int)TimeSpan.FromDays(7).TotalMilliseconds;

    // A API (publicadora) e o worker (consumidor) declaram a mesma topologia: a declaracao e idempotente,
    // mas falha se os argumentos divergirem, por isso ela vive num lugar so.
    public static async Task DeclareAsync(IChannel channel, CancellationToken cancellationToken = default)
    {
        await channel.QueueDeclareAsync(
            DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?> { ["x-message-ttl"] = DeadLetterTtlMilliseconds },
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            SendQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = string.Empty,
                ["x-dead-letter-routing-key"] = DeadLetterQueue
            },
            cancellationToken: cancellationToken);
    }
}
