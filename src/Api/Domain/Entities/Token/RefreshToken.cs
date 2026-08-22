using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Api.Domain.Entities.BaseEntities;
using Recipes.Api.Domain.Entities.Users;

namespace Recipes.Api.Domain.Entities.Token;

public class RefreshToken : BaseEntity
{
    public RefreshToken(string token, DateTime expiresAt, User user)
    {
        Token = token;
        ExpiresAt = expiresAt;
        UserId = user.Id;
    }

    [Required] [MaxLength(255)] public string Token { get; init; } = null!;
    public DateTime ExpiresAt { get; init; }
    [ForeignKey("User")] [Required] public int UserId { get; init; }
    public virtual User? User { get; init; }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }
}