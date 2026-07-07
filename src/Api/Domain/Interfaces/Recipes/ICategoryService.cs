using Recipes.Api.Domain.DTOs.Recipes;

namespace Recipes.Api.Domain.Interfaces.Recipes;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAll(int page, int size);
    Task<CategoryResponse?> GetById(int id);
    Task<CategoryResponse?> Create(CreateCategoryRequest request);
    Task<CategoryResponse?> Update(int id, UpdateCategoryRequest request);
    Task<bool> Disable(int id);
}
