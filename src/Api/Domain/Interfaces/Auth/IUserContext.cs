namespace Recipes.Api.Domain.Interfaces.Auth;

public interface IUserContext
{
    int? UserId { get; }
    int GetUserId();
}
