using Recipes.Api.Domain.Entities.Admin;

namespace Recipes.Api.Domain.Interfaces.Recipes;

public interface IUnitRepository
{
    Task<IEnumerable<Unit>> GetAll(int page, int size);
    Task<Unit?> GetById(int id);
    Task<Unit?> Create(Unit unit);
    Task<Unit?> Update(Unit unit);
}
