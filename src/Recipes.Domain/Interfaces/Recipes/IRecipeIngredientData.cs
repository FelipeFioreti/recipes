namespace Recipes.Domain.Interfaces.Recipes;

public interface IRecipeIngredientData
{
    int? Id { get; }
    string Name { get; }
    decimal Quantity { get; }
    int UnitId { get; }
}
