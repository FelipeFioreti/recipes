using Recipes.Api.Domain.DTOs.Recipes;

namespace Recipes.Api.Domain.Interfaces.Recipes;

public interface IUnitService
{
    Task<IEnumerable<UnitResponse>> GetAll(int page, int size);
    Task<UnitResponse?> GetById(int id);
    Task<UnitResponse?> Create(CreateUnitRequest request);
    Task<UnitResponse?> Update(int id, UpdateUnitRequest request);
    Task<bool> Disable(int id);
}
