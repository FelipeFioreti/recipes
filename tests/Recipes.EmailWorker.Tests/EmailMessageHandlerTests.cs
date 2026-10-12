using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Recipes.EmailWorker.Smtp;
using Recipes.Messaging.Email;

namespace Recipes.EmailWorker.Tests;

public class EmailMessageHandlerTests
{
    private static readonly SendEmailMessage Mensagem =
        new("msg-1", "felipe@exemplo.com", "Assunto", "<p>corpo</p>");

    private readonly EmailMessageHandler _handler;
    private readonly IEmailSender _sender = Substitute.For<IEmailSender>();

    public EmailMessageHandlerTests()
    {
        _handler = new EmailMessageHandler(
            _sender,
            new EmailRetrySettings { Delays = [TimeSpan.Zero, TimeSpan.Zero] },
            NullLogger<EmailMessageHandler>.Instance);
    }

    [Fact]
    public async Task Handle_MensagemValida_EnviaEConfirma()
    {
        var resultado = await _handler.Handle(Serializar(Mensagem), CancellationToken.None);

        Assert.Equal(EmailHandlingResult.Ack, resultado);
        await _sender.Received(1).Send("felipe@exemplo.com", "Assunto", "<p>corpo</p>", Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("isso nao e json")]
    [InlineData("""{"MessageId":"x","To":"","Subject":"s","HtmlBody":"b"}""")]
    [InlineData("""{"MessageId":"x","To":"a@b.com","Subject":"s"}""")]
    public async Task Handle_MensagemInvalida_VaiParaDeadLetterSemEnviar(string corpo)
    {
        var resultado = await _handler.Handle(Encoding.UTF8.GetBytes(corpo), CancellationToken.None);

        Assert.Equal(EmailHandlingResult.DeadLetter, resultado);
        await _sender.DidNotReceiveWithAnyArgs().Send(default!, default!, default!);
    }

    [Fact]
    public async Task Handle_FalhaTransitoria_TentaDeNovoEConfirma()
    {
        _sender.Send(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new IOException("smtp caiu"), _ => Task.CompletedTask);

        var resultado = await _handler.Handle(Serializar(Mensagem), CancellationToken.None);

        Assert.Equal(EmailHandlingResult.Ack, resultado);
        await _sender.ReceivedWithAnyArgs(2).Send(default!, default!, default!);
    }

    [Fact]
    public async Task Handle_FalhaEmTodasAsTentativas_VaiParaDeadLetter()
    {
        _sender.Send(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new IOException("smtp fora do ar"));

        var resultado = await _handler.Handle(Serializar(Mensagem), CancellationToken.None);

        Assert.Equal(EmailHandlingResult.DeadLetter, resultado);
        await _sender.ReceivedWithAnyArgs(3).Send(default!, default!, default!);
    }

    [Fact]
    public async Task Handle_Cancelado_PropagaSemDecidir()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        _sender.Send(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _handler.Handle(Serializar(Mensagem), cts.Token));
    }

    [Theory]
    [InlineData("felipe@exemplo.com", "f***@exemplo.com")]
    [InlineData("sem-arroba", "***")]
    public void MaskEmail_EscondeOUsuario(string email, string esperado)
    {
        Assert.Equal(esperado, EmailMessageHandler.MaskEmail(email));
    }

    private static byte[] Serializar(SendEmailMessage mensagem) => JsonSerializer.SerializeToUtf8Bytes(mensagem);
}
