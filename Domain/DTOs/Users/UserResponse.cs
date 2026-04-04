using Recipes.Domain.Entities.Enums;
using Recipes.Domain.Entities.Users;

namespace Recipes.Domain.DTOs.Users;

public record UserResponse
{
    public UserResponse(User user)
    {
        Id = user.Id;
        Name = user.Name;
        Email = user.Email;
        Role = user.Role;
        CreatedAt = user.CreatedAt;
        UpdatedAt = user.UpdatedAt;
        DeletedAt = user.DeletedAt;
    }

    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public Roles Role { get; init; } = Roles.USER;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}