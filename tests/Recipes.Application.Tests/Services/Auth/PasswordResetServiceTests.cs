using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Recipes.Application.DTOs.Auth;
using Recipes.Application.Interfaces.Auth;
using Recipes.Application.Interfaces.Email;
using Recipes.Application.Services.Auth;
using Recipes.Application.Settings;
using Recipes.Domain.Entities.Token;
using Recipes.Domain.Entities.Users;
using Recipes.Domain.Interfaces.Token;
using Recipes.Domain.Interfaces.Users;

namespace Recipes.Application.Tests.Services.Auth;

public class PasswordResetServiceTests
{
    private const string ResetUrl = "https://chefarchive.test/reset-password";

    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IPasswordResetTokenRepository _resetTokenRepository =
        Substitute.For<IPasswordResetTokenRepository>();
    private readonly PasswordResetService _service;
    private readonly User _user = new("Felipe", "felipe@exemplo.com", "hash-antigo");
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    public PasswordResetServiceTests()
    {
        _service = new PasswordResetService(
            _userRepository,
            _resetTokenRepository,
            _refreshTokenRepository,
            _passwordService,
            _emailService,
            Options.Create(new AppSettings { PasswordResetUrl = ResetUrl, PasswordResetTokenExpirationMinutes = 30 }),
            NullLogger<PasswordResetService>.Instance);
    }

    [Fact]
    public async Task RequestReset_EmailInexistente_NaoCriaTokenNemEnviaEmail()
    {
        _userRepository.GetByEmail(Arg.Any<string>()).Returns((User?)null);

        await _service.RequestReset(new ForgotPasswordRequest { Email = "ninguem@exemplo.com" });

        await _resetTokenRepository.DidNotReceive().Create(Arg.Any<PasswordResetToken>());
        await _emailService.DidNotReceiveWithAnyArgs().Send(default!, default!, default!);
    }

    [Fact]
    public async Task RequestReset_NormalizaEmailParaMinusculas()
    {
        await _service.RequestReset(new ForgotPasswordRequest { Email = "Felipe@EXEMPLO.com" });

        await _userRepository.Received(1).GetByEmail("felipe@exemplo.com");
    }

    [Fact]
    public async Task RequestReset_PersisteSoOHashEEnviaOTokenNoLink()
    {
        _userRepository.GetByEmail("felipe@exemplo.com").Returns(_user);
        PasswordResetToken? criado = null;
        _resetTokenRepository.Create(Arg.Do<PasswordResetToken>(t => criado = t)).Returns(c => c.Arg<PasswordResetToken>());
        string? corpo = null;
        await _emailService.Send("felipe@exemplo.com", Arg.Any<string>(), Arg.Do<string>(b => corpo = b));

        await _service.RequestReset(new ForgotPasswordRequest { Email = "felipe@exemplo.com" });

        Assert.NotNull(criado);
        Assert.NotNull(corpo);
        var token = ExtrairToken(corpo);
        Assert.NotEqual(token, criado.TokenHash);
        Assert.Equal(Hash(token), criado.TokenHash);
        Assert.InRange(criado.ExpiresAt, DateTime.UtcNow.AddMinutes(29), DateTime.UtcNow.AddMinutes(31));
        await _resetTokenRepository.Received(1).DeleteByUser(_user.Id);
    }

    [Fact]
    public async Task RequestReset_FalhaNoEnvioDeEmail_NaoPropagaExcecao()
    {
        _userRepository.GetByEmail(Arg.Any<string>()).Returns(_user);
        _emailService.Send(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new InvalidOperationException("smtp fora do ar"));

        await _service.RequestReset(new ForgotPasswordRequest { Email = "felipe@exemplo.com" });

        await _resetTokenRepository.Received(1).Create(Arg.Any<PasswordResetToken>());
    }

    [Fact]
    public async Task ResetPassword_TokenValido_TrocaSenhaEEncerraSessoes()
    {
        var resetToken = new PasswordResetToken(Hash("token-valido"), DateTime.UtcNow.AddMinutes(10), _user);
        _resetTokenRepository.GetByHash(Hash("token-valido")).Returns(resetToken);
        _userRepository.GetById(_user.Id).Returns(_user);
        _passwordService.HashPassword("nova-senha").Returns("hash-novo");

        var ok = await _service.ResetPassword(new ResetPasswordRequest
            { Token = "token-valido", NewPassword = "nova-senha" });

        Assert.True(ok);
        Assert.Equal("hash-novo", _user.PasswordHash);
        Assert.True(resetToken.IsUsed());
        await _userRepository.Received(1).Update(_user);
        await _refreshTokenRepository.Received(1).DeleteByUser(_user.Id);
        await _resetTokenRepository.Received(1).DeleteByUser(_user.Id);
    }

    [Fact]
    public async Task ResetPassword_TokenInexistente_RetornaFalse()
    {
        _resetTokenRepository.GetByHash(Arg.Any<string>()).Returns((PasswordResetToken?)null);

        Assert.False(await _service.ResetPassword(new ResetPasswordRequest
            { Token = "qualquer", NewPassword = "nova-senha" }));
        await _userRepository.DidNotReceive().Update(Arg.Any<User>());
    }

    [Fact]
    public async Task ResetPassword_TokenExpirado_RetornaFalseSemTrocarSenha()
    {
        var expirado = new PasswordResetToken(Hash("expirado"), DateTime.UtcNow.AddMinutes(-1), _user);
        _resetTokenRepository.GetByHash(Hash("expirado")).Returns(expirado);

        Assert.False(await _service.ResetPassword(new ResetPasswordRequest
            { Token = "expirado", NewPassword = "nova-senha" }));
        Assert.Equal("hash-antigo", _user.PasswordHash);
        await _refreshTokenRepository.DidNotReceive().DeleteByUser(Arg.Any<int>());
    }

    [Fact]
    public async Task ResetPassword_TokenJaUsado_RetornaFalse()
    {
        var usado = new PasswordResetToken(Hash("usado"), DateTime.UtcNow.AddMinutes(10), _user);
        usado.MarkUsed();
        _resetTokenRepository.GetByHash(Hash("usado")).Returns(usado);

        Assert.False(await _service.ResetPassword(new ResetPasswordRequest
            { Token = "usado", NewPassword = "nova-senha" }));
        Assert.Equal("hash-antigo", _user.PasswordHash);
    }

    private static string ExtrairToken(string corpo)
    {
        const string marcador = ResetUrl + "?token=";
        var inicio = corpo.IndexOf(marcador, StringComparison.Ordinal) + marcador.Length;
        var fim = corpo.IndexOf('"', inicio);
        return corpo[inicio..fim];
    }

    private static string Hash(string token)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
