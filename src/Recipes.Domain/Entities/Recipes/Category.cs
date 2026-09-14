using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Domain.Entities.Base;

namespace Recipes.Domain.Entities.Recipes;

[Table("Categories")]
public class Category : BaseEntity
{
    public Category()
    {
    }

    public Category(string name)
    {
        Name = name;
    }

    [MaxLength(255)] [Required] public string Name { get; set; } = string.Empty;
    public ICollection<Recipe>? Recipes { get; set; } = new List<Recipe>();

    public void Update(string name)
    {
        Name = name;
    }
}
