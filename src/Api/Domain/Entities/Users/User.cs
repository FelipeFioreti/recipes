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

    public User(string name, string email, string passwordHash)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = Roles.USER;
    }

    [MaxLength(255)] [Required] public string Name { get; private set; } = string.Empty;
    [MaxLength(255)] [Required] public string Email { get; private set; } = string.Empty;

    [MaxLength(255)]
    [Required]
    [JsonIgnore]
    public string PasswordHash { get; private set; } = string.Empty;

    [MaxLength(50)] [Required] public Roles Role { get; private set; }

    public ICollection<Recipe>? Recipes { get; set; } = new List<Recipe>();

    public void Update(UpdateUserRequest request)
    {
        Name = request.Name;
    }

    public void ChangePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }
}