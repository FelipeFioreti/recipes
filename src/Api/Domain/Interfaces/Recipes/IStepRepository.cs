using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Domain.Interfaces.Recipes;

public interface IStepRepository
{
    Task<IEnumerable<Step>> GetAll(int page, int size);
    Task<IEnumerable<Step>> GetAllForUser(int userId, int page, int size);
    Task<Step?> GetById(int id);
    Task<Step?> GetByIdForUser(int id, int userId);
    Task<Step?> Create(Step step);
    Task<Step?> Update(Step step);
}
