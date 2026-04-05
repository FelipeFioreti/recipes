using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Domain.DTOs.Recipes;
using Recipes.Domain.Entities.BaseEntities;
using Recipes.Domain.Entities.Users;

namespace Recipes.Domain.Entities.Recipes;

[Table("Recipes")]
public class Recipe : BaseEntity
{
    public Recipe()
    {
    }

    public Recipe(string name, string description, int recipeTypeId, int userId)
    {
        Name = name;
        Description = description;
        RecipeTypeId = recipeTypeId;
        UserId = userId;
    }

    [MaxLength(255)] [Required] public string Name { get; private set; } = string.Empty;
    [MaxLength(2000)] public string Description { get; private set; } = string.Empty;
    [ForeignKey("RecipeType")] [Required] public int RecipeTypeId { get; private set; }
    [ForeignKey("User")] [Required] public int UserId { get; private set; }

    public User User { get; set; } = null!;
    public RecipeType RecipeType { get; set; } = null!;

    public void Update(UpdateRecipeRequest request)
    {
        Name = request.Name;
        Description = request.Description;
        RecipeTypeId = request.RecipeTypeId;
    }
}