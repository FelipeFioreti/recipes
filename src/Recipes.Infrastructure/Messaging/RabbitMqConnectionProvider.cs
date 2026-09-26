using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Recipes.Messaging;
using Recipes.Messaging.Email;

namespace Recipes.Infrastructure.Messaging;

public sealed class RabbitMqConnectionProvider(
    IOptions<RabbitMqSettings> rabbitMqSettings,
    ILogger<RabbitMqConnectionProvider> logger) : IAsyncDisposable
{
    private const string ClientName = "chefarchive-api";

    private readonly SemaphoreSlim _lock = new(1, 1);
    private IConnection? _connection;

    public async Task<IConnection> GetConnection(CancellationToken cancellationToken = default)
    {
        if (_connection is { IsOpen: true })
            return _connection;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true })
                return _connection;

            if (_connection != null)
                await _connection.DisposeAsync();

            logger.LogDebug("GetConnection()");

            var connection = await rabbitMqSettings.Value
                .CreateConnectionFactory(ClientName)
                .CreateConnectionAsync(cancellationToken);

            await using (var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken))
            {
                await EmailTopology.DeclareAsync(channel, cancellationToken);
            }

            _connection = connection;
            return connection;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
            await _connection.DisposeAsync();

        _lock.Dispose();
    }
}
