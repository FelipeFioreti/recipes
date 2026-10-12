using System.Security.Cryptography;
using System.Text;

namespace Recipes.Application.Services.Auth;

internal static class TokenHasher
{
    public static string Hash(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);
        return Convert.ToBase64String(hashBytes);
    }
}
