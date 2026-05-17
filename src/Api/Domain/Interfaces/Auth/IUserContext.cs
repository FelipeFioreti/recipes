namespace Recipes.Api.Domain.Interfaces.Auth;

public interface IUserContext
{
    int GetUserId();
    bool IsAdmin();
}