using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Recipes.Api.Domain.Entities.BaseEntities;

namespace Recipes.Api.Domain.Entities.Recipes;

[Table("Steps")]
public class Step : BaseEntity
{
    [SetsRequiredMembers]
    public Step()
    {
        Description = string.Empty;
    }

    [SetsRequiredMembers]
    public Step(int recipeId, int position, string description)
    {
        RecipeId = recipeId;
        Position = position;
        Description = description;
    }

    [SetsRequiredMembers]
    public Step(int? id, int recipeId, int position, string description)
        : this(recipeId, position, description)
    {
        if (id.HasValue)
            Id = id.Value;
    }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public int Position { get; set; }

    [MaxLength(2000)] [Required] public required string Description { get; set; }
}
