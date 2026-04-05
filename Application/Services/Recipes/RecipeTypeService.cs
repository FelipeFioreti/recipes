using Recipes.Domain.DTOs.Recipes;
using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Application.Services.Recipes;

public class RecipeTypeService(IRecipeTypeRepository recipeTypeRepository, ILogger<RecipeTypeService> logger)
    : IRecipeTypeService
{
    public async Task<IEnumerable<RecipeTypeResponse>> GetAll()
    {
        logger.LogDebug("GetAll()");

        var recipeTypes = await recipeTypeRepository.GetAll();

        return recipeTypes.Select(ToResponse);
    }

    public async Task<RecipeTypeResponse?> GetById(int id)
    {
        logger.LogDebug("GetById()");

        var recipeType = await recipeTypeRepository.GetById(id);

        return recipeType == null ? null : ToResponse(recipeType);
    }

    public async Task<RecipeTypeResponse?> Create(CreateRecipeTypeRequest request)
    {
        logger.LogDebug("Create()");

        var recipeType = await recipeTypeRepository.Create(new RecipeType(request.Name));

        return recipeType == null ? null : ToResponse(recipeType);
    }

    public async Task<RecipeTypeResponse?> Update(int id, UpdateRecipeTypeRequest request)
    {
        logger.LogDebug("Update()");

        var existingRecipeType = await recipeTypeRepository.GetById(id);

        if (existingRecipeType == null)
            return null;

        existingRecipeType.Update(request);

        var updatedRecipeType = await recipeTypeRepository.Update(existingRecipeType);

        return updatedRecipeType == null ? null : ToResponse(updatedRecipeType);
    }

    public async Task<bool> Disable(int id)
    {
        logger.LogDebug("Disable()");

        var recipeType = await recipeTypeRepository.GetById(id);

        if (recipeType == null)
            return false;

        recipeType.Disable();
        await recipeTypeRepository.Update(recipeType);

        return true;
    }

    private static RecipeTypeResponse ToResponse(RecipeType recipeType)
    {
        return new RecipeTypeResponse(recipeType);
    }
}
