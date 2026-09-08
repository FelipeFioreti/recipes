using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Domain.Entities.Admin;
using Recipes.Domain.Entities.Base;

namespace Recipes.Domain.Entities.Recipes;

[Table("Ingredients")]
public class Ingredient : BaseEntity
{
    public Ingredient()
    {
    }

    public Ingredient(string name, decimal quantity, int recipeId, int unitId)
    {
        Name = name;
        Quantity = quantity;
        RecipeId = recipeId;
        UnitId = unitId;
    }

    public Ingredient(int? id, string name, decimal quantity, int recipeId, int unitId)
        : this(name, quantity, recipeId, unitId)
    {
        if (id.HasValue)
            Id = id.Value;
    }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    [MaxLength(255)] [Required] public string Name { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    public void Update(string name, decimal quantity, int unitId)
    {
        Name = name;
        Quantity = quantity;
        UnitId = unitId;
    }
}
