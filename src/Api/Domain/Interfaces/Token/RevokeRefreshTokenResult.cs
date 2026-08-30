using Recipes.Api.Domain.Entities.Token;

namespace Recipes.Api.Domain.Interfaces.Token;

public record RevokeRefreshTokenResult(RefreshToken? RefreshToken, bool WasRevoked);
