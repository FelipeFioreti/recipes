using Recipes.Domain.DTOs.Recipes;

namespace Recipes.Domain.Interfaces.Recipes;

public interface IRecipeTypeService
{
    Task<IEnumerable<RecipeTypeResponse>> GetAll();
    Task<RecipeTypeResponse?> GetById(int id);
    Task<RecipeTypeResponse?> Create(CreateRecipeTypeRequest request);
    Task<RecipeTypeResponse?> Update(int id, UpdateRecipeTypeRequest request);
    Task<bool> Disable(int id);
}
