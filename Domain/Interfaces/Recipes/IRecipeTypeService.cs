using Recipes.Domain.DTOs.Recipes;

namespace Recipes.Domain.Interfaces.Recipes;

public interface IRecipeTypeService
{
    Task<IEnumerable<RecipeTypeResponse>> GetAll();
    Task<RecipeTypeResponse?> GetById(int id);
    Task<RecipeTypeResponse?> Create(CreateRecipeTypeRequest request);
    Task<RecipeTypeResponse?> Update(UpdateRecipeTypeRequest request);
    Task<bool> Disable(int id);
    Task<bool> Delete(int id);
}
