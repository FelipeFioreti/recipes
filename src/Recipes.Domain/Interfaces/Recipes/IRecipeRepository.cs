using Recipes.Domain.Entities.Recipes;

namespace Recipes.Domain.Interfaces.Recipes;

public interface IRecipeRepository
{
    Task<IEnumerable<Recipe>> GetAll(int page = 0, int pageSize = 10);
    Task<IEnumerable<Recipe>> GetAllForUser(int userId, int page = 0, int pageSize = 10);
    Task<Recipe?> GetByIdForUser(int id, int userId, bool tracked = false);
    Task<Recipe?> GetById(int id, bool tracked = false);
    Task<Recipe?> Create(Recipe recipe);
    Task<Recipe?> Update(Recipe recipe);
    Task<bool> CanAccessRecipe(int recipeId, int userId);
}
