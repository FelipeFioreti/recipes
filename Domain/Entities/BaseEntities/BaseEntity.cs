using System.ComponentModel.DataAnnotations;

namespace Recipes.Domain.Entities.BaseEntities;

public abstract class BaseEntity
{
    [Key] public int Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    private void Delete()
    {
        DeletedAt = DateTime.UtcNow;
        Update();
    }

    private void Update()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}