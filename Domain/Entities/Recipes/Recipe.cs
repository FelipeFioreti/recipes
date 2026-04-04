using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Domain.Entities.BaseEntities;
using Recipes.Domain.Entities.Users;

namespace Recipes.Domain.Entities.Recipes;

public class Recipe : BaseEntity
{
    [MaxLength(255)] [Required] public string Name { get; set; } = string.Empty;
    [MaxLength(2000)] public string Description { get; set; } = string.Empty;
    [ForeignKey("RecipeType")] [Required] public int RecipeTypeId { get; set; }
    [ForeignKey("User")] [Required] public int UserId { get; set; }

    public User User { get; set; } = null!;
    public RecipeType RecipeType { get; set; } = null!;
}