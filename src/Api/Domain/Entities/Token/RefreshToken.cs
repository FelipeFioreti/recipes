using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Api.Domain.Entities.BaseEntities;
using Recipes.Api.Domain.Entities.Users;

namespace Recipes.Api.Domain.Entities.Token;

public class RefreshToken : BaseEntity
{
    private RefreshToken()
    {
    }

    public RefreshToken(string tokenHash, DateTime expiresAt, User user)
    {
        TokenHash = tokenHash;
        RevokedAt = null;
        ExpiresAt = expiresAt;
        UserId = user.Id;
    }

    [Required] [MaxLength(255)] public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    [ForeignKey("User")] [Required] public int UserId { get; private set; }
    public virtual User? User { get; private set; }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
    public void Revoke() => RevokedAt = DateTime.UtcNow;
    public bool IsRevoked() => RevokedAt.HasValue;
}
