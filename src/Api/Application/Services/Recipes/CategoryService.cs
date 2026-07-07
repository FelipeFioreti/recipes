using Recipes.Api.Domain.DTOs.Recipes;
using Recipes.Api.Domain.Entities.Recipes;
using Recipes.Api.Domain.Interfaces.Recipes;

namespace Recipes.Api.Application.Services.Recipes;

public class CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
    : ICategoryService
{
    public async Task<IEnumerable<CategoryResponse>> GetAll(int page, int size)
    {
        logger.LogDebug("GetAll()");

        var categories = await categoryRepository.GetAll(page, size);

        return categories.Select(ToResponse);
    }

    public async Task<CategoryResponse?> GetById(int id)
    {
        logger.LogDebug("GetById()");

        var category = await categoryRepository.GetById(id);

        return category == null ? null : ToResponse(category);
    }

    public async Task<CategoryResponse?> Create(CreateCategoryRequest request)
    {
        logger.LogDebug("Create()");

        var category = await categoryRepository.Create(new Category(request.Name));

        return category == null ? null : ToResponse(category);
    }

    public async Task<CategoryResponse?> Update(int id, UpdateCategoryRequest request)
    {
        logger.LogDebug("Update()");

        var existingCategory = await categoryRepository.GetById(id);

        if (existingCategory == null)
            return null;

        existingCategory.Update(request);

        var updatedCategory = await categoryRepository.Update(existingCategory);

        return updatedCategory == null ? null : ToResponse(updatedCategory);
    }

    public async Task<bool> Disable(int id)
    {
        logger.LogDebug("Disable()");

        var category = await categoryRepository.GetById(id);

        if (category == null)
            return false;

        category.Disable();
        await categoryRepository.Update(category);

        return true;
    }

    private static CategoryResponse ToResponse(Category category)
    {
        return new CategoryResponse(category);
    }
}
