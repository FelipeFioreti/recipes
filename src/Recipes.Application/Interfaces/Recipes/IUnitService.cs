using Recipes.Application.DTOs.Recipes;

namespace Recipes.Application.Interfaces.Recipes;

public interface IUnitService
{
    Task<IEnumerable<UnitResponse>> GetAll(int page, int size);
    Task<UnitResponse?> GetById(int id);
    Task<UnitResponse?> Create(CreateUnitRequest request);
    Task<UnitResponse?> Update(UpdateUnitRequest request);
    Task<bool> Disable(int id);
}
