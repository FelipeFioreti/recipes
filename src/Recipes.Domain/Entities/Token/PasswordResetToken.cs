using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Domain.Entities.Base;
using Recipes.Domain.Entities.Users;

namespace Recipes.Domain.Entities.Token;

public class PasswordResetToken : BaseEntity
{
    private PasswordResetToken()
    {
    }

    public PasswordResetToken(string tokenHash, DateTime expiresAt, User user)
    {
        TokenHash = tokenHash;
        UsedAt = null;
        ExpiresAt = expiresAt;
        UserId = user.Id;
    }

    [Required] [MaxLength(255)] public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    [ForeignKey("User")] [Required] public int UserId { get; private set; }
    public virtual User? User { get; private set; }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
    public void MarkUsed() => UsedAt = DateTime.UtcNow;
    public bool IsUsed() => UsedAt.HasValue;
    public bool IsValid() => !IsUsed() && !IsExpired();
}
