using Recipes.Application.DTOs.Recipes;
using Recipes.Application.Mappings;
using Recipes.Domain.Entities.Recipes;

namespace Recipes.Application.Tests.Mappings;

public class RecipeMappingsTests
{
    [Fact]
    public void ToEntity_ProjetaFilhosDoRequestSemConversaoIntermediaria()
    {
        var request = new CreateRecipeRequest
        {
            Name = "Torta",
            Description = "Descricao",
            CategoryId = 4,
            Ingredients =
            [
                new RecipeIngredientRequest { Name = "Leite", Quantity = 500m, UnitId = 3 },
                new RecipeIngredientRequest { Name = "Ovo", Quantity = 2m, UnitId = 1 }
            ],
            Steps = [new RecipeStepRequest { Description = "Bater", Position = 1 }]
        };

        var recipe = request.ToEntity(userId: 7);

        Assert.Equal("Torta", recipe.Name);
        Assert.Equal(4, recipe.CategoryId);
        Assert.Equal(7, recipe.UserId);
        Assert.Equal(["Leite", "Ovo"], recipe.Ingredients.Select(ingredient => ingredient.Name));
        Assert.Equal(500m, recipe.Ingredients.First().Quantity);
        Assert.Equal("Bater", Assert.Single(recipe.Steps).Description);
    }

    [Fact]
    public void ApplyTo_PreservaIdsExistentesEDesativaAusentes()
    {
        var recipe = new Recipe();
        recipe.Ingredients.Add(new Ingredient(1, "Farinha", 200m, 99, 1));
        recipe.Ingredients.Add(new Ingredient(2, "Acucar", 100m, 99, 1));

        new UpdateRecipeRequest
        {
            Id = 99,
            Name = "Bolo",
            Description = "d",
            CategoryId = 5,
            Ingredients = [new RecipeIngredientRequest { Id = 1, Name = "Farinha integral", Quantity = 250m, UnitId = 1 }],
            Steps = []
        }.ApplyTo(recipe);

        var mantido = recipe.Ingredients.Single(ingredient => ingredient.Id == 1);
        Assert.Equal("Farinha integral", mantido.Name);
        Assert.Null(mantido.DeletedAt);
        Assert.NotNull(recipe.Ingredients.Single(ingredient => ingredient.Id == 2).DeletedAt);
    }

    [Fact]
    public void ApplyTo_QuandoIngredientesOmitidosNaoDesativaNenhum()
    {
        var recipe = new Recipe();
        recipe.Ingredients.Add(new Ingredient(1, "Farinha", 200m, 99, 1));
        recipe.Ingredients.Add(new Ingredient(2, "Acucar", 100m, 99, 1));

        new UpdateRecipeRequest
        {
            Id = 99,
            Name = "Bolo renomeado",
            Description = "d",
            CategoryId = 5
        }.ApplyTo(recipe);

        Assert.Equal("Bolo renomeado", recipe.Name);
        Assert.All(recipe.Ingredients, ingredient => Assert.Null(ingredient.DeletedAt));
    }

    [Fact]
    public void ToResponse_MapeiaCamposEOrdenaPassosPorPosicao()
    {
        var recipe = new Recipe("Bolo", "Descricao", 5, 7, [], []);
        recipe.Steps.Add(new Step(11, 99, 3, "Assar"));
        recipe.Steps.Add(new Step(10, 99, 1, "Misturar"));
        recipe.Ingredients.Add(new Ingredient(1, "Farinha", 200m, 99, 2));

        var response = recipe.ToResponse();

        Assert.Equal("Bolo", response.Name);
        Assert.Equal("Descricao", response.Description);
        Assert.Equal(5, response.CategoryId);
        Assert.Equal(7, response.UserId);
        Assert.Null(response.Category);
        Assert.Equal(["Misturar", "Assar"], response.Steps.Select(step => step.Description));
        Assert.Equal("Farinha", Assert.Single(response.Ingredients).Name);
    }
}
