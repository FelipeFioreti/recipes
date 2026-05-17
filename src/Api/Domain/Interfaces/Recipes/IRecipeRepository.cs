using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Domain.Interfaces.Recipes;

public interface IRecipeRepository
{
    Task<IEnumerable<Recipe>> GetAll();
    Task<IEnumerable<Recipe>> GetAllForUser(int userId);
    Task<Recipe?> GetByIdForUser(int id, int userId);
    Task<Recipe?> GetById(int id);
    Task<Recipe?> Create(Recipe recipe);
    Task<Recipe?> Update(Recipe recipe);
}