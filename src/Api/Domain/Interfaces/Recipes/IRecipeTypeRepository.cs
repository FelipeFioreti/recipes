using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Domain.Interfaces.Recipes;

public interface IRecipeTypeRepository
{
    Task<IEnumerable<RecipeType>> GetAll();
    Task<RecipeType?> GetById(int id);
    Task<RecipeType?> Create(RecipeType recipeType);
    Task<RecipeType?> Update(RecipeType recipeType);
}