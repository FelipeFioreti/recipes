using Recipes.Application.DTOs.Recipes;
using Recipes.Domain.Entities.Admin;

namespace Recipes.Application.Mappings;

public static class UnitMappings
{
    public static UnitResponse ToResponse(this Unit unit)
    {
        return new UnitResponse(unit);
    }
}
