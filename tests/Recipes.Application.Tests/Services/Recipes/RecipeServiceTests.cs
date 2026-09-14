using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Recipes.Application.DTOs.Recipes;
using Recipes.Application.Interfaces.Auth;
using Recipes.Application.Services.Recipes;
using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Application.Tests.Services.Recipes;

public class RecipeServiceTests
{
    private const int UserId = 7;

    private readonly IRecipeRepository _repository = Substitute.For<IRecipeRepository>();
    private readonly RecipeService _service;
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    public RecipeServiceTests()
    {
        _userContext.GetUserId().Returns(UserId);
        _service = new RecipeService(_repository, _userContext, NullLogger<RecipeService>.Instance);
    }

    [Fact]
    public async Task GetAll_QuandoAdmin_BuscaTodasAsReceitas()
    {
        _userContext.IsAdmin().Returns(true);
        _repository.GetAll(1, 10).Returns([new Recipe()]);

        var resultado = await _service.GetAll(1, 10);

        Assert.Single(resultado);
        await _repository.Received(1).GetAll(1, 10);
        await _repository.DidNotReceiveWithAnyArgs().GetAllForUser(default, default, default);
    }

    [Fact]
    public async Task GetAll_QuandoNaoAdmin_RestringeAoUsuarioAutenticado()
    {
        _userContext.IsAdmin().Returns(false);
        _repository.GetAllForUser(UserId, 1, 10).Returns([new Recipe()]);

        await _service.GetAll(1, 10);

        await _repository.Received(1).GetAllForUser(UserId, 1, 10);
        await _repository.DidNotReceiveWithAnyArgs().GetAll(default, default);
    }

    [Fact]
    public async Task GetById_QuandoNaoAdmin_RestringeAoUsuarioAutenticado()
    {
        _userContext.IsAdmin().Returns(false);
        _repository.GetByIdForUser(3, UserId, false).Returns(new Recipe());

        var resultado = await _service.GetById(3);

        Assert.NotNull(resultado);
        await _repository.Received(1).GetByIdForUser(3, UserId, false);
    }

    [Fact]
    public async Task GetById_QuandoNaoEncontra_RetornaNull()
    {
        _userContext.IsAdmin().Returns(true);
        _repository.GetById(3, false).Returns((Recipe?)null);

        Assert.Null(await _service.GetById(3));
    }

    [Fact]
    public async Task Create_UsaOIdDoUsuarioAutenticado()
    {
        _repository.Create(Arg.Any<Recipe>()).Returns(callInfo => callInfo.Arg<Recipe>());

        var resultado = await _service.Create(new CreateRecipeRequest
        {
            Name = "Torta",
            Description = "Descricao",
            CategoryId = 4,
            Ingredients = [new RecipeIngredientRequest { Name = "Leite", Quantity = 500m, UnitId = 3 }],
            Steps = [new RecipeStepRequest { Description = "Bater", Position = 1 }]
        });

        var criada = _repository.ReceivedCalls()
            .Single(call => call.GetMethodInfo().Name == nameof(IRecipeRepository.Create))
            .GetArguments()[0] as Recipe;

        Assert.NotNull(criada);
        Assert.Equal(UserId, criada.UserId);
        Assert.Equal("Torta", criada.Name);
        Assert.Single(criada.Ingredients);
        Assert.Single(criada.Steps);
        Assert.Equal("Torta", resultado?.Name);
    }

    [Fact]
    public async Task Update_QuandoReceitaNaoExiste_RetornaNullSemPersistir()
    {
        _userContext.IsAdmin().Returns(true);
        _repository.GetById(3, true).Returns((Recipe?)null);

        Assert.Null(await _service.Update(new UpdateRecipeRequest { Id = 3, Name = "Novo" }));
        await _repository.DidNotReceiveWithAnyArgs().Update(default!);
    }

    [Fact]
    public async Task Update_CarregaComTrackingEAplicaNaMesmaInstancia()
    {
        var existente = new Recipe("Antigo", "d", 1, UserId, [], []);
        _userContext.IsAdmin().Returns(false);
        _repository.GetByIdForUser(3, UserId, true).Returns(existente);
        _repository.Update(Arg.Any<Recipe>()).Returns(callInfo => callInfo.Arg<Recipe>());

        var resultado = await _service.Update(new UpdateRecipeRequest
        {
            Id = 3, Name = "Renomeada", Description = "Nova", CategoryId = 9
        });

        Assert.Equal("Renomeada", existente.Name);
        Assert.Equal(9, existente.CategoryId);
        Assert.Equal("Renomeada", resultado?.Name);
        await _repository.Received(1).Update(existente);
    }

    [Fact]
    public async Task Disable_MarcaDeletedAtEPersiste()
    {
        var receita = new Recipe();
        _userContext.IsAdmin().Returns(true);
        _repository.GetById(3, true).Returns(receita);

        Assert.True(await _service.Disable(3));
        Assert.NotNull(receita.DeletedAt);
        await _repository.Received(1).Update(receita);
    }

    [Fact]
    public async Task Disable_QuandoNaoEncontra_RetornaFalseSemPersistir()
    {
        _userContext.IsAdmin().Returns(true);
        _repository.GetById(3, true).Returns((Recipe?)null);

        Assert.False(await _service.Disable(3));
        await _repository.DidNotReceiveWithAnyArgs().Update(default!);
    }
}
