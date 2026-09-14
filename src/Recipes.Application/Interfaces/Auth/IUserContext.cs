namespace Recipes.Application.Interfaces.Auth;

public interface IUserContext
{
    int GetUserId();
    bool IsAdmin();
}
