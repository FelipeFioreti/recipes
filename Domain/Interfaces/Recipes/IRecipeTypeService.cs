using Recipes.Domain.Entities.Recipes;

namespace Recipes.Domain.Interfaces.Recipes;

public interface IRecipeTypeService
{
    Task<IEnumerable<RecipeType>> GetAll();
    Task<RecipeType?> GetById(int id);
    Task<RecipeType?> Create(RecipeType recipeType);
    Task<RecipeType?> Update(RecipeType recipeType);
    Task Disable(int id);
    Task Delete(int id);
}