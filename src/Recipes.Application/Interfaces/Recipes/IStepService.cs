using Recipes.Application.DTOs.Recipes;

namespace Recipes.Application.Interfaces.Recipes;

public interface IStepService
{
    Task<IEnumerable<StepResponse>> GetAll(int page, int size);
    Task<StepResponse?> GetById(int id);
    Task<StepResponse?> Create(CreateStepRequest request);
    Task<StepResponse?> Update(UpdateStepRequest request);
    Task<bool> Disable(int id);
}
