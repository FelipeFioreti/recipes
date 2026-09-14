using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Recipes.Application.DTOs.Users;
using Recipes.Application.Interfaces.Auth;
using Recipes.Application.Services.Users;
using Recipes.Domain.Entities.Users;
using Recipes.Domain.Interfaces.Users;

namespace Recipes.Application.Tests.Services.Users;

public class UserServiceTests
{
    private const int UserId = 7;

    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly UserService _service;
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    public UserServiceTests()
    {
        _userContext.GetUserId().Returns(UserId);
        _service = new UserService(_repository, _userContext, _passwordService,
            NullLogger<UserService>.Instance);
    }

    [Fact]
    public async Task Create_NormalizaEmailParaMinusculas()
    {
        _passwordService.HashPassword(Arg.Any<string>()).Returns("hash");
        _repository.Create(Arg.Any<User>()).Returns(callInfo => callInfo.Arg<User>());

        await _service.Create(new CreateUserRequest("Felipe", "Felipe@EXEMPLO.com", "senha123"));

        Assert.Equal("felipe@exemplo.com", UsuarioCriado().Email);
    }

    [Fact]
    public async Task Create_PersisteOHashNuncaASenhaEmClaro()
    {
        _passwordService.HashPassword("senha123").Returns("$2a$11$hash-fake");
        _repository.Create(Arg.Any<User>()).Returns(callInfo => callInfo.Arg<User>());

        await _service.Create(new CreateUserRequest("Felipe", "felipe@exemplo.com", "senha123"));

        var criado = UsuarioCriado();
        Assert.Equal("$2a$11$hash-fake", criado.PasswordHash);
        Assert.NotEqual("senha123", criado.PasswordHash);
        _passwordService.Received(1).HashPassword("senha123");
    }

    [Fact]
    public async Task Create_QuandoRepositorioNaoPersiste_RetornaNull()
    {
        _passwordService.HashPassword(Arg.Any<string>()).Returns("hash");
        _repository.Create(Arg.Any<User>()).Returns((User?)null);

        Assert.Null(await _service.Create(new CreateUserRequest("Felipe", "f@e.com", "senha123")));
    }

    [Fact]
    public async Task GetCurrent_ResolvePeloIdDoContexto()
    {
        _repository.GetById(UserId).Returns(new User("Felipe", "f@e.com", "hash"));

        var resultado = await _service.GetCurrent();

        Assert.Equal("Felipe", resultado?.Name);
        await _repository.Received(1).GetById(UserId);
    }

    [Fact]
    public async Task Update_QuandoUsuarioNaoExiste_RetornaNullSemPersistir()
    {
        _repository.GetById(3).Returns((User?)null);

        Assert.Null(await _service.Update(new UpdateUserRequest { Id = 3, Name = "Novo" }));
        await _repository.DidNotReceiveWithAnyArgs().Update(default!);
    }

    [Fact]
    public async Task Update_AlteraApenasONome()
    {
        var existente = new User("Felipe", "felipe@exemplo.com", "hash-original");
        _repository.GetById(3).Returns(existente);
        _repository.Update(Arg.Any<User>()).Returns(callInfo => callInfo.Arg<User>());

        var resultado = await _service.Update(new UpdateUserRequest { Id = 3, Name = "Felipe Fioreti" });

        Assert.Equal("Felipe Fioreti", existente.Name);
        Assert.Equal("felipe@exemplo.com", existente.Email);
        Assert.Equal("hash-original", existente.PasswordHash);
        Assert.Equal("Felipe Fioreti", resultado?.Name);
    }

    [Fact]
    public async Task Disable_MarcaDeletedAtEPersiste()
    {
        var usuario = new User("Felipe", "f@e.com", "hash");
        _repository.GetById(3).Returns(usuario);

        Assert.True(await _service.Disable(3));
        Assert.NotNull(usuario.DeletedAt);
        await _repository.Received(1).Update(usuario);
    }

    private User UsuarioCriado()
    {
        var criado = _repository.ReceivedCalls()
            .Single(call => call.GetMethodInfo().Name == nameof(IUserRepository.Create))
            .GetArguments()[0] as User;

        Assert.NotNull(criado);
        return criado;
    }
}
