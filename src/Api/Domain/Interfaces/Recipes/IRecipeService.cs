using Recipes.Api.Domain.DTOs.Recipes;

namespace Recipes.Api.Domain.Interfaces.Recipes;

public interface IRecipeService
{
    Task<IEnumerable<RecipeResponse>> GetAll();
    Task<RecipeResponse?> GetById(int id);
    Task<RecipeResponse?> Create(CreateRecipeRequest request);
    Task<RecipeResponse?> Update(int id, UpdateRecipeRequest request);
    Task<bool> Disable(int id);
}
