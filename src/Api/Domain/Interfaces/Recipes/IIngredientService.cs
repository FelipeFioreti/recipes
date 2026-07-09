using Recipes.Api.Domain.DTOs.Recipes;

namespace Recipes.Api.Domain.Interfaces.Recipes;

public interface IIngredientService
{
    Task<IEnumerable<IngredientResponse>> GetAll(int page, int size);
    Task<IngredientResponse?> GetById(int id);
    Task<IngredientResponse?> Create(CreateIngredientRequest request);
    Task<IngredientResponse?> Update(int id, UpdateIngredientRequest request);
    Task<bool> Disable(int id);
}
