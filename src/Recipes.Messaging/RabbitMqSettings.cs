using RabbitMQ.Client;

namespace Recipes.Messaging;

public class RabbitMqSettings
{
    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string Username { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string VirtualHost { get; init; } = "/";

    public ConnectionFactory CreateConnectionFactory(string clientName)
    {
        return new ConnectionFactory
        {
            HostName = Host,
            Port = Port,
            UserName = Username,
            Password = Password,
            VirtualHost = VirtualHost,
            ClientProvidedName = clientName,
            AutomaticRecoveryEnabled = true,
            RequestedConnectionTimeout = TimeSpan.FromSeconds(5)
        };
    }
}
