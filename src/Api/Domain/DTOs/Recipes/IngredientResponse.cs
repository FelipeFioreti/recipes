using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record IngredientResponse
{
    public IngredientResponse(Ingredient ingredient)
    {
        Id = ingredient.Id;
        RecipeId = ingredient.RecipeId;
        Name = ingredient.Name;
        Quantity = ingredient.Quantity;
        UnitId = ingredient.UnitId;
        Unit = ingredient.Unit is null
            ? null
            : new UnitResponse(ingredient.Unit);
        CreatedAt = ingredient.CreatedAt;
        UpdatedAt = ingredient.UpdatedAt;
        DeletedAt = ingredient.DeletedAt;
    }

    public int Id { get; init; }
    public int RecipeId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public int UnitId { get; init; }
    public UnitResponse? Unit { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}
