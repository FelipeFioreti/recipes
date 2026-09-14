using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Domain.Entities.Base;

namespace Recipes.Domain.Entities.Admin;

[Table("Units")]
public class Unit : BaseEntity
{
    public Unit()
    {
    }

    public Unit(string name, bool showAbbreviation, string abbreviation)
    {
        Name = name;
        ShowAbbreviation = showAbbreviation;
        Abbreviation = abbreviation;
    }


    [MaxLength(50)] [Required] public string Name { get; set; } = string.Empty;
    [Required] public bool ShowAbbreviation { get; set; }

    [MaxLength(10)] public string Abbreviation { get; set; } = string.Empty;

    public void Update(string name, bool showAbbreviation, string abbreviation)
    {
        Name = name;
        ShowAbbreviation = showAbbreviation;
        Abbreviation = abbreviation;
    }
}
