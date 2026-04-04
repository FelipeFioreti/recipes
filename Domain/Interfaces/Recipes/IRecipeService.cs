using Recipes.Domain.Entities.Recipes;

namespace Recipes.Domain.Interfaces.Recipes;

public interface IRecipeService
{
    Task<IEnumerable<Recipe>> GetAll(int userId);
    Task<Recipe?> GetById(int id, int userId);
    Task<Recipe?> Create(int userId, Recipe recipe);
    Task<Recipe?> Update(int userId, Recipe recipe);
    Task<bool> Disable(int id, int userId);
    Task<bool> Delete(int id, int userId);
}
