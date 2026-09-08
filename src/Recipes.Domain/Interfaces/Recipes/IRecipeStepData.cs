namespace Recipes.Domain.Interfaces.Recipes;

public interface IRecipeStepData
{
    int? Id { get; }
    string Description { get; }
    int Position { get; }
}
