using Recipes.Domain.Entities.Recipes;

namespace Recipes.Domain.Interfaces.Recipes;

public interface IRecipeService
{
    Task<IEnumerable<Recipe>> GetAll();
    Task<Recipe?> GetById(int id);
    Task<Recipe?> Create(Recipe recipe);
    Task<Recipe?> Update(Recipe recipe);
    Task Disable(int id);
    Task Delete(int id);
}