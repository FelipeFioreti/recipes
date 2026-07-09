using Recipes.Api.Domain.Entities.Admin;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record UnitResponse
{
    public UnitResponse(Unit unit)
    {
        Id = unit.Id;
        Name = unit.Name;
        Abbreviation = unit.Abbreviation;
        CreatedAt = unit.CreatedAt;
        UpdatedAt = unit.UpdatedAt;
        DeletedAt = unit.DeletedAt;
    }

    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Abbreviation { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}
