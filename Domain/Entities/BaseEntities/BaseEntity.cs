using System.ComponentModel.DataAnnotations;

namespace Recipes.Domain.Entities.BaseEntities;

public abstract class BaseEntity
{
    [Key] public int Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }

    public void Disable()
    {
        DeletedAt = DateTime.UtcNow;
        Update();
    }

    protected virtual void Update()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}