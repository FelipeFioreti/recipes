using Recipes.Domain.Entities.Token;

namespace Recipes.Domain.Interfaces.Token;

public record RevokeRefreshTokenResult(RefreshToken? RefreshToken, bool WasRevoked);
