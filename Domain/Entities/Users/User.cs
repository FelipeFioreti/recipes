using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Recipes.Domain.Entities.BaseEntities;
using Recipes.Domain.Entities.Enums;
using Recipes.Domain.Entities.Recipes;

namespace Recipes.Domain.Entities.Users;

public class User : BaseEntity
{
    [MaxLength(255)] [Required] public string Name { get; set; } = string.Empty;
    [MaxLength(255)] [Required] public string Email { get; set; } = string.Empty;

    [MaxLength(255)]
    [Required]
    [JsonIgnore]
    public string Password { get; set; } = string.Empty;

    [MaxLength(50)] [Required] public Roles Role { get; set; } = Roles.USER;

    public ICollection<Recipe>? Recipes { get; set; } = new List<Recipe>();
}