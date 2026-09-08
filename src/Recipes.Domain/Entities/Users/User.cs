using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Recipes.Domain.Entities.Base;
using Recipes.Domain.Entities.Enums;
using Recipes.Domain.Entities.Recipes;
using Recipes.Domain.Entities.Token;

namespace Recipes.Domain.Entities.Users;

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

    public ICollection<Recipe>? Recipes { get; init; } = new List<Recipe>();
    public ICollection<RefreshToken>? RefreshTokens { get; init; } = new List<RefreshToken>();

    public void Update(string name)
    {
        Name = name;
    }

    public void ChangePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }
}
