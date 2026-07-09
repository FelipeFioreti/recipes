using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Domain.Interfaces.Recipes;

public interface IRecipeRepository
{
    Task<IEnumerable<Recipe>> GetAll(int page = 1, int pageSize = 10);
    Task<IEnumerable<Recipe>> GetAllForUser(int userId, int page = 1, int pageSize = 10);
    Task<Recipe?> GetByIdForUser(int id, int userId);
    Task<Recipe?> GetById(int id);
    Task<Recipe?> Create(Recipe recipe);
    Task<Recipe?> Update(Recipe recipe);
    Task<bool> CanAccessRecipe(int recipeId, int userId);
}