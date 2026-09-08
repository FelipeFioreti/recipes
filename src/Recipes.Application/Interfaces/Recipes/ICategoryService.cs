using Recipes.Application.DTOs.Recipes;

namespace Recipes.Application.Interfaces.Recipes;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAll(int page, int size);
    Task<CategoryResponse?> GetById(int id);
    Task<CategoryResponse?> Create(CreateCategoryRequest request);
    Task<CategoryResponse?> Update(UpdateCategoryRequest request);
    Task<bool> Disable(int id);
}
