using Recipes.Api.Domain.DTOs.Recipes;
using Recipes.Api.Domain.Entities.Recipes;
using Recipes.Api.Domain.Interfaces.Auth;
using Recipes.Api.Domain.Interfaces.Recipes;

namespace Recipes.Api.Application.Services.Recipes;

public class StepService(
    IStepRepository stepRepository,
    IRecipeRepository recipeRepository,
    IUserContext userContext,
    ILogger<StepService> logger) : IStepService
{
    public async Task<IEnumerable<StepResponse>> GetAll(int page, int size)
    {
        logger.LogDebug("GetAll()");

        var steps = userContext.IsAdmin()
            ? await stepRepository.GetAll(page, size)
            : await stepRepository.GetAllForUser(userContext.GetUserId(), page, size);

        return steps.Select(ToResponse);
    }

    public async Task<StepResponse?> GetById(int id)
    {
        logger.LogDebug("GetById()");

        var step = userContext.IsAdmin()
            ? await stepRepository.GetById(id)
            : await stepRepository.GetByIdForUser(id, userContext.GetUserId());

        return step == null ? null : ToResponse(step);
    }

    public async Task<StepResponse?> Create(CreateStepRequest request)
    {
        logger.LogDebug("Create()");

        var hasAccessToRecipe = await CanAccessRecipe(request.RecipeId);

        if (!hasAccessToRecipe)
            return null;

        var step = await stepRepository.Create(new Step(request.RecipeId, request.Position, request.Description));

        return step == null ? null : ToResponse(step);
    }

    public async Task<StepResponse?> Update(int id, UpdateStepRequest request)
    {
        logger.LogDebug("Update()");

        var existingStep = userContext.IsAdmin()
            ? await stepRepository.GetById(id)
            : await stepRepository.GetByIdForUser(id, userContext.GetUserId());

        if (existingStep == null)
            return null;

        var hasAccessToRecipe = await CanAccessRecipe(request.RecipeId);

        if (!hasAccessToRecipe)
            return null;

        existingStep.RecipeId = request.RecipeId;
        existingStep.Position = request.Position;
        existingStep.Description = request.Description;

        var updatedStep = await stepRepository.Update(existingStep);

        return updatedStep == null ? null : ToResponse(updatedStep);
    }

    public async Task<bool> Disable(int id)
    {
        logger.LogDebug("Disable()");

        var step = userContext.IsAdmin()
            ? await stepRepository.GetById(id)
            : await stepRepository.GetByIdForUser(id, userContext.GetUserId());

        if (step == null)
            return false;

        step.Disable();
        await stepRepository.Update(step);

        return true;
    }

    private async Task<bool> CanAccessRecipe(int recipeId)
    {
        if (userContext.IsAdmin())
            return await recipeRepository.GetById(recipeId) != null;

        return await recipeRepository.GetByIdForUser(recipeId, userContext.GetUserId()) != null;
    }

    private static StepResponse ToResponse(Step step)
    {
        return new StepResponse(step);
    }
}
