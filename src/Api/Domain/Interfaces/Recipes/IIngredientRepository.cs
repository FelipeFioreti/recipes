using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Domain.Interfaces.Recipes;

public interface IIngredientRepository
{
    Task<IEnumerable<Ingredient>> GetAll(int page, int size);
    Task<IEnumerable<Ingredient>> GetAllForUser(int userId, int page, int size);
    Task<Ingredient?> GetById(int id);
    Task<Ingredient?> GetByIdForUser(int id, int userId);
    Task<Ingredient?> Create(Ingredient ingredient);
    Task<Ingredient?> Update(Ingredient ingredient);
}
