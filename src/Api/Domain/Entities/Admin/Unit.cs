using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Api.Domain.Entities.BaseEntities;

namespace Recipes.Api.Domain.Entities.Admin;

[Table("Units")]
public class Unit : BaseEntity
{
    public Unit()
    {
    }

    public Unit(string name, string abbreviation)
    {
        Name = name;
        Abbreviation = abbreviation;
    }


    [MaxLength(50)] [Required] public string Name { get; set; } = string.Empty;

    [MaxLength(10)] public string Abbreviation { get; set; } = string.Empty;
}