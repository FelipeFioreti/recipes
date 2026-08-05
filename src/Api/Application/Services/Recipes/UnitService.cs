using Recipes.Api.Domain.DTOs.Recipes;
using Recipes.Api.Domain.Entities.Admin;
using Recipes.Api.Domain.Interfaces.Recipes;

namespace Recipes.Api.Application.Services.Recipes;

public class UnitService(IUnitRepository unitRepository, ILogger<UnitService> logger) : IUnitService
{
    public async Task<IEnumerable<UnitResponse>> GetAll(int page, int size)
    {
        logger.LogDebug("GetAll()");

        var units = await unitRepository.GetAll(page, size);

        return units.Select(unit => unit.ToResponse());
    }

    public async Task<UnitResponse?> GetById(int id)
    {
        logger.LogDebug("GetById()");

        var unit = await unitRepository.GetById(id);

        return unit?.ToResponse();
    }

    public async Task<UnitResponse?> Create(CreateUnitRequest request)
    {
        logger.LogDebug("Create()");

        var unit = await unitRepository.Create(new Unit(request.Name, request.ShowAbbreviation, request.Abbreviation));

        return unit?.ToResponse();
    }

    public async Task<UnitResponse?> Update(UpdateUnitRequest request)
    {
        logger.LogDebug("Update()");

        if(await unitRepository.GetById(request.Id) is not {} existingUnit)
            return null;

        existingUnit.Update(request);

        var updatedUnit = await unitRepository.Update(existingUnit);

        return updatedUnit?.ToResponse();
    }

    public async Task<bool> Disable(int id)
    {
        logger.LogDebug("Disable()");

        var unit = await unitRepository.GetById(id);

        if (unit == null)
            return false;

        unit.Disable();
        await unitRepository.Update(unit);

        return true;
    }
}