using System.ComponentModel.DataAnnotations;

namespace Recipes.Application.DTOs.Recipes;

public record UpdateRecipeRequest
{
    public int Id { get; init; }

    [Required] [MaxLength(255)] public string Name { get; init; } = string.Empty;

    [MaxLength(2000)] public string Description { get; init; } = string.Empty;

    [Required] public int CategoryId { get; init; }

    /// Null = nao mexer nos ingredientes existentes. Lista (mesmo vazia) = substituir por completo,
    /// desativando quem nao aparecer.
    public ICollection<RecipeIngredientRequest>? Ingredients { get; init; }

    /// Null = nao mexer nos passos existentes. Lista (mesmo vazia) = substituir por completo,
    /// desativando quem nao aparecer.
    public ICollection<RecipeStepRequest>? Steps { get; init; }
}
