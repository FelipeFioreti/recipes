using Recipes.Application.DTOs.Recipes;
using Recipes.Application.Interfaces.Recipes;
using Recipes.Application.Mappings;
using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Application.Services.Recipes;

public class CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
    : ICategoryService
{
    public async Task<IEnumerable<CategoryResponse>> GetAll(int page, int size)
    {
        logger.LogDebug("GetAll()");

        var categories = await categoryRepository.GetAll(page, size);

        return categories.Select(category => category.ToResponse());
    }

    public async Task<CategoryResponse?> GetById(int id)
    {
        logger.LogDebug("GetById()");

        var category = await categoryRepository.GetById(id);

        return category == null ? null : category.ToResponse();
    }

    public async Task<CategoryResponse?> Create(CreateCategoryRequest request)
    {
        logger.LogDebug("Create()");

        var category = await categoryRepository.Create(new Category(request.Name));

        return category == null ? null : category.ToResponse();
    }

    public async Task<CategoryResponse?> Update(UpdateCategoryRequest request)
    {
        logger.LogDebug("Update()");

        if (await categoryRepository.GetById(request.Id) is not { } existingCategory)
            return null;

        existingCategory.Update(request.Name);

        var updatedCategory = await categoryRepository.Update(existingCategory);

        return updatedCategory == null ? null : updatedCategory.ToResponse();
    }

    public async Task<bool> Disable(int id)
    {
        logger.LogDebug("Disable()");

        if (await categoryRepository.GetById(id) is not { } category)
            return false;

        category.Disable();
        await categoryRepository.Update(category);

        return true;
    }
}
