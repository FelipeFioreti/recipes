using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Domain.Tests.Entities.Recipes;

public class RecipeTests
{
    private const int RecipeId = 99;

    [Fact]
    public void Construtor_AssociaFilhosAReceita()
    {
        var recipe = new Recipe(
            "Bolo", "Descricao", 5, 7,
            [new IngredientData(null, "Farinha", 200m, 1)],
            [new StepData(null, "Misturar", 1)]);

        Assert.Equal("Bolo", recipe.Name);
        Assert.Equal(5, recipe.CategoryId);
        Assert.Equal(7, recipe.UserId);
        Assert.Same(recipe, Assert.Single(recipe.Ingredients).Recipe);
        Assert.Same(recipe, Assert.Single(recipe.Steps).Recipe);
    }

    [Fact]
    public void Update_AtualizaCamposEscalares()
    {
        var recipe = RecipeComFilhos();

        recipe.Update("Bolo novo", "Outra descricao", 42, [], []);

        Assert.Equal("Bolo novo", recipe.Name);
        Assert.Equal("Outra descricao", recipe.Description);
        Assert.Equal(42, recipe.CategoryId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void Update_AdicionaIngredienteSemIdPersistido(int? id)
    {
        var recipe = RecipeComFilhos();

        recipe.Update("Bolo", "d", 1, [new IngredientData(id, "Ovo", 3m, 2)], []);

        var novo = Assert.Single(recipe.Ingredients, ingredient => ingredient.Name == "Ovo");
        Assert.Equal(0, novo.Id);
        Assert.Equal(3m, novo.Quantity);
        Assert.Equal(2, novo.UnitId);
        Assert.Same(recipe, novo.Recipe);
        Assert.Null(novo.DeletedAt);
    }

    [Fact]
    public void Update_AtualizaIngredienteExistenteSemRecriar()
    {
        var recipe = RecipeComFilhos();
        var original = recipe.Ingredients.Single(ingredient => ingredient.Id == 1);

        recipe.Update("Bolo", "d", 1, [new IngredientData(1, "Farinha integral", 250m, 4)], []);

        var atualizado = Assert.Single(recipe.Ingredients, ingredient => ingredient.Id == 1);
        Assert.Same(original, atualizado);
        Assert.Equal("Farinha integral", atualizado.Name);
        Assert.Equal(250m, atualizado.Quantity);
        Assert.Equal(4, atualizado.UnitId);
        Assert.Null(atualizado.DeletedAt);
    }

    [Fact]
    public void Update_DesativaIngredienteAusenteNoPayload()
    {
        var recipe = RecipeComFilhos();

        recipe.Update("Bolo", "d", 1, [new IngredientData(1, "Farinha", 200m, 1)], []);

        var removido = recipe.Ingredients.Single(ingredient => ingredient.Id == 2);
        Assert.NotNull(removido.DeletedAt);
        Assert.Null(recipe.Ingredients.Single(ingredient => ingredient.Id == 1).DeletedAt);
    }

    [Fact]
    public void Update_NaoDesativaIngredienteRecemAdicionado()
    {
        var recipe = RecipeComFilhos();

        recipe.Update("Bolo", "d", 1,
            [new IngredientData(1, "Farinha", 200m, 1), new IngredientData(null, "Ovo", 3m, 2)],
            []);

        Assert.Null(recipe.Ingredients.Single(ingredient => ingredient.Name == "Ovo").DeletedAt);
    }

    [Fact]
    public void Update_LancaQuandoIngredienteNaoPertenceAReceita()
    {
        var recipe = RecipeComFilhos();

        var erro = Assert.Throws<KeyNotFoundException>(() =>
            recipe.Update("Bolo", "d", 1, [new IngredientData(999, "Fantasma", 1m, 1)], []));

        Assert.Contains("999", erro.Message);
    }

    [Fact]
    public void Update_DesativaPassoAusenteNoPayload()
    {
        var recipe = RecipeComFilhos();

        recipe.Update("Bolo", "d", 1, [], [new StepData(10, "Misturar bem", 1)]);

        var atualizado = recipe.Steps.Single(step => step.Id == 10);
        Assert.Equal("Misturar bem", atualizado.Description);
        Assert.Null(atualizado.DeletedAt);
        Assert.NotNull(recipe.Steps.Single(step => step.Id == 11).DeletedAt);
    }

    [Fact]
    public void Update_AdicionaPassoSemId()
    {
        var recipe = RecipeComFilhos();

        recipe.Update("Bolo", "d", 1, [],
            [new StepData(10, "Misturar", 1), new StepData(null, "Decorar", 3)]);

        var novo = Assert.Single(recipe.Steps, step => step.Description == "Decorar");
        Assert.Equal(0, novo.Id);
        Assert.Equal(3, novo.Position);
        Assert.Same(recipe, novo.Recipe);
    }

    [Fact]
    public void Update_LancaQuandoPassoNaoPertenceAReceita()
    {
        var recipe = RecipeComFilhos();

        Assert.Throws<KeyNotFoundException>(() =>
            recipe.Update("Bolo", "d", 1, [], [new StepData(555, "Fantasma", 1)]));
    }

    [Fact]
    public void Update_ComPayloadVazioDesativaTodosOsFilhos()
    {
        var recipe = RecipeComFilhos();

        recipe.Update("Bolo", "d", 1, [], []);

        Assert.All(recipe.Ingredients, ingredient => Assert.NotNull(ingredient.DeletedAt));
        Assert.All(recipe.Steps, step => Assert.NotNull(step.DeletedAt));
    }

    [Fact]
    public void Update_ComIngredientesENullosNaoTocaNosFilhosExistentes()
    {
        var recipe = RecipeComFilhos();

        recipe.Update("Bolo novo", "d", 1, null, null);

        Assert.All(recipe.Ingredients, ingredient => Assert.Null(ingredient.DeletedAt));
        Assert.All(recipe.Steps, step => Assert.Null(step.DeletedAt));
        Assert.Equal(2, recipe.Ingredients.Count);
        Assert.Equal(2, recipe.Steps.Count);
    }

    /// Estado equivalente ao que o repositorio devolve: so filhos vivos, com ids ja persistidos.
    private static Recipe RecipeComFilhos()
    {
        var recipe = new Recipe();

        recipe.Ingredients.Add(new Ingredient(1, "Farinha", 200m, RecipeId, 1));
        recipe.Ingredients.Add(new Ingredient(2, "Acucar", 100m, RecipeId, 1));
        recipe.Steps.Add(new Step(10, RecipeId, 1, "Misturar"));
        recipe.Steps.Add(new Step(11, RecipeId, 2, "Assar"));

        return recipe;
    }

    private sealed record IngredientData(int? Id, string Name, decimal Quantity, int UnitId)
        : IRecipeIngredientData;

    private sealed record StepData(int? Id, string Description, int Position) : IRecipeStepData;
}
