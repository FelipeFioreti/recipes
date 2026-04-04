using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Application.Services.Recipes;

public class RecipeTypeService(IRecipeTypeRepository recipeTypeRepository, ILogger<RecipeTypeService> logger)
    : IRecipeTypeService
{
    public async Task<IEnumerable<RecipeType>> GetAll()
    {
        logger.LogDebug("GetAll()");

        return await recipeTypeRepository.GetAll();
    }

    public async Task<RecipeType?> GetById(int id)
    {
        logger.LogDebug("GetById()");

        return await recipeTypeRepository.GetById(id);
    }

    public async Task<RecipeType?> Create(RecipeType recipeType)
    {
        logger.LogDebug("Create()");

        return await recipeTypeRepository.Create(recipeType);
    }

    public async Task<RecipeType?> Update(RecipeType recipeType)
    {
        logger.LogDebug("Update()");

        return await recipeTypeRepository.Update(recipeType);
    }

    public async Task Disable(int id)
    {
        logger.LogDebug("Disable()");

        var recipeType = await GetById(id);

        if (recipeType == null)
            return;

        recipeType.DeletedAt = DateTime.Now;
        await recipeTypeRepository.Update(recipeType);
    }

    public async Task Delete(int id)
    {
        logger.LogDebug("Delete()");

        var recipeType = await GetById(id);

        if (recipeType == null)
            return;

        await recipeTypeRepository.Delete(recipeType);
    }
}
