namespace Recipes.Api.Domain.Interfaces.Auth;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
