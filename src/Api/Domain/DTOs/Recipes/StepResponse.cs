using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Domain.DTOs.Recipes;

public record StepResponse
{
    public StepResponse(Step step)
    {
        Id = step.Id;
        RecipeId = step.RecipeId;
        Position = step.Position;
        Description = step.Description;
        CreatedAt = step.CreatedAt;
        UpdatedAt = step.UpdatedAt;
        DeletedAt = step.DeletedAt;
    }

    public int Id { get; init; }
    public int RecipeId { get; init; }
    public int Position { get; init; }
    public string Description { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}
