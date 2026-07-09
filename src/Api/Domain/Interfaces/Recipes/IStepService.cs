using Recipes.Api.Domain.DTOs.Recipes;

namespace Recipes.Api.Domain.Interfaces.Recipes;

public interface IStepService
{
    Task<IEnumerable<StepResponse>> GetAll(int page, int size);
    Task<StepResponse?> GetById(int id);
    Task<StepResponse?> Create(CreateStepRequest request);
    Task<StepResponse?> Update(int id, UpdateStepRequest request);
    Task<bool> Disable(int id);
}
