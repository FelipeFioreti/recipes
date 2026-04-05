using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Domain.DTOs.Recipes;
using Recipes.Domain.Entities.BaseEntities;

namespace Recipes.Domain.Entities.Recipes;

[Table("RecipeTypes")]
public class RecipeType : BaseEntity
{
    public RecipeType()
    {
    }

    public RecipeType(string name)
    {
        Name = name;
    }

    [MaxLength(255)] [Required] public string Name { get; set; } = string.Empty;
    public ICollection<Recipe>? Recipes { get; set; } = new List<Recipe>();

    public void Update(UpdateRecipeTypeRequest request)
    {
        Name = request.Name;
    }
}