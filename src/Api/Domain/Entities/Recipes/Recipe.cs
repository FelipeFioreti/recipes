using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Api.Domain.DTOs.Recipes;
using Recipes.Api.Domain.Entities.BaseEntities;
using Recipes.Api.Domain.Entities.Users;

namespace Recipes.Api.Domain.Entities.Recipes;

[Table("Recipes")]
public class Recipe : BaseEntity
{
    public Recipe()
    {
    }

    public Recipe(string name, string description, int categoryId, int userId)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        UserId = userId;
    }

    [MaxLength(255)] [Required] public string Name { get; private set; } = string.Empty;
    [MaxLength(2000)] public string Description { get; private set; } = string.Empty;
    [ForeignKey("Category")] [Required] public int CategoryId { get; private set; }
    [ForeignKey("User")] [Required] public int UserId { get; private set; }

    public User User { get; set; } = null!;
    public Category? Category { get; set; } = null!;

    public void Update(UpdateRecipeRequest request)
    {
        Name = request.Name;
        Description = request.Description;
        CategoryId = request.CategoryId;
    }
}
