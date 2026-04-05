using Recipes.Domain.DTOs.Recipes;

namespace Recipes.Domain.Interfaces.Recipes;

public interface IRecipeService
{
    Task<IEnumerable<RecipeResponse>> GetAll(int userId);
    Task<RecipeResponse?> GetById(int id, int userId);
    Task<RecipeResponse?> Create(int userId, CreateRecipeRequest request);
    Task<RecipeResponse?> Update(int userId, UpdateRecipeRequest request);
    Task<bool> Disable(int id, int userId);
    Task<bool> Delete(int id, int userId);
}
