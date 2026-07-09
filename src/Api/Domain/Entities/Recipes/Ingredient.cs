using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Api.Domain.DTOs.Recipes;
using Recipes.Api.Domain.Entities.Admin;
using Recipes.Api.Domain.Entities.BaseEntities;

namespace Recipes.Api.Domain.Entities.Recipes;

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

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    [MaxLength(255)] [Required] public string Name { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    public void Update(UpdateIngredientRequest request)
    {
        Name = request.Name;
        Quantity = request.Quantity;
        RecipeId = request.RecipeId;
        UnitId = request.UnitId;
    }
}