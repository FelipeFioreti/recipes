using Recipes.Application.DTOs.Recipes;

namespace Recipes.Application.Interfaces.Recipes;

public interface IIngredientService
{
    Task<IEnumerable<IngredientResponse>> GetAll(int page, int size);
    Task<IngredientResponse?> GetById(int id);
    Task<IngredientResponse?> Create(CreateIngredientRequest request);
    Task<IngredientResponse?> Update(UpdateIngredientRequest request);
    Task<bool> Disable(int id);
}
