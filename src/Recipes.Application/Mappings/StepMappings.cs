using Recipes.Application.DTOs.Recipes;
using Recipes.Domain.Entities.Recipes;

namespace Recipes.Application.Mappings;

public static class StepMappings
{
    public static StepResponse ToResponse(this Step step)
    {
        return new StepResponse(step);
    }
}
