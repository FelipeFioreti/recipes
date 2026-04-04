using System.ComponentModel.DataAnnotations;
using Recipes.Domain.Entities.BaseEntities;

namespace Recipes.Domain.Entities.Recipes;

public class RecipeType : BaseEntity
{
    [MaxLength(255)] [Required] public string Name { get; set; } = string.Empty;
    public ICollection<Recipe>? Recipes { get; set; } = new List<Recipe>();
}