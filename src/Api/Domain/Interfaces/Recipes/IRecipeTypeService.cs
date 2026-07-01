using Recipes.Api.Domain.DTOs.Recipes;

namespace Recipes.Api.Domain.Interfaces.Recipes;

public interface IRecipeTypeService
{
    Task<IEnumerable<RecipeTypeResponse>> GetAll(int page, int size);
    Task<RecipeTypeResponse?> GetById(int id);
    Task<RecipeTypeResponse?> Create(CreateRecipeTypeRequest request);
    Task<RecipeTypeResponse?> Update(int id, UpdateRecipeTypeRequest request);
    Task<bool> Disable(int id);
}