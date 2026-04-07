using System.ComponentModel.DataAnnotations;

namespace Recipes.Api.Domain.Entities.BaseEntities;

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

    private void Update()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCreatedAt()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public void SetUpdatedAt()
    {
        Update();
    }
}