using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Recipes.Api.Domain.DTOs.Users;
using Recipes.Api.Domain.Entities.BaseEntities;
using Recipes.Api.Domain.Entities.Enums;
using Recipes.Api.Domain.Entities.Recipes;

namespace Recipes.Api.Domain.Entities.Users;

[Table("Users")]
public class User : BaseEntity
{
    public User()
    {
    }

    public User(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
        Role = Roles.USER;
    }

    [MaxLength(255)] [Required] public string Name { get; private set; } = string.Empty;
    [MaxLength(255)] [Required] public string Email { get; private set; } = string.Empty;

    [MaxLength(255)]
    [Required]
    [JsonIgnore]
    public string Password { get; private set; } = string.Empty;

    [MaxLength(50)] [Required] public Roles Role { get; private set; }

    public ICollection<Recipe>? Recipes { get; set; } = new List<Recipe>();

    public void Update(UpdateUserRequest request)
    {
        Name = request.Name;
    }

    public void ChangePassword(string passwordHash)
    {
        Password = passwordHash;
    }
}
