using Recipes.Domain.Entities.Recipes;

namespace Recipes.Domain.Interfaces.Recipes;

public interface IRecipeRepository
{
    Task<IEnumerable<Recipe>> GetAll();
    Task<Recipe?> GetById(int id);
    Task<Recipe?> Create(Recipe recipe);
    Task<Recipe?> Update(Recipe recipe);
    Task Delete(Recipe recipe);
}