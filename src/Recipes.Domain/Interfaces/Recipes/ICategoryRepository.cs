using Recipes.Domain.Entities.Recipes;

namespace Recipes.Domain.Interfaces.Recipes;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAll(int page, int size);
    Task<Category?> GetById(int id);
    Task<Category?> Create(Category category);
    Task<Category?> Update(Category category);
}
