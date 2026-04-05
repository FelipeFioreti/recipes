using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Recipes.Domain.DTOs.Users;
using Recipes.Domain.Entities.BaseEntities;
using Recipes.Domain.Entities.Enums;
using Recipes.Domain.Entities.Recipes;

namespace Recipes.Domain.Entities.Users;

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
        Password = HashPassword(password);
        Role = Roles.USER;
    }

    [MaxLength(255)] [Required] public string Name { get; private set; }
    [MaxLength(255)] [Required] public string Email { get; private set; }

    [MaxLength(255)]
    [Required]
    [JsonIgnore]
    public string Password { get; private set; }

    [MaxLength(50)] [Required] public Roles Role { get; private set; }

    public ICollection<Recipe>? Recipes { get; set; } = new List<Recipe>();

    public void Update(UpdateUserRequest request)
    {
        Name = request.Name;
        if (!string.IsNullOrWhiteSpace(request.Password))
            Password = HashPassword(request.Password);
    }

    private static string HashPassword(string password)
    {
        return password.StartsWith("$2") ? password : BCrypt.Net.BCrypt.HashPassword(password);
    }
}