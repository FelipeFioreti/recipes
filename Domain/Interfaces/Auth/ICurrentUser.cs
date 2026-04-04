namespace Recipes.Domain.Interfaces.Auth;

public interface ICurrentUser
{
    int? UserId { get; }
}