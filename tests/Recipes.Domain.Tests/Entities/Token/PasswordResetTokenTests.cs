using Recipes.Domain.Entities.Token;
using Recipes.Domain.Entities.Users;

namespace Recipes.Domain.Tests.Entities.Token;

public class PasswordResetTokenTests
{
    private readonly User _user = new("Felipe", "felipe@exemplo.com", "hash");

    [Fact]
    public void TokenNovoDentroDoPrazo_EhValido()
    {
        var token = new PasswordResetToken("hash", DateTime.UtcNow.AddMinutes(30), _user);

        Assert.False(token.IsExpired());
        Assert.False(token.IsUsed());
        Assert.True(token.IsValid());
    }

    [Fact]
    public void TokenExpirado_NaoEhValido()
    {
        var token = new PasswordResetToken("hash", DateTime.UtcNow.AddSeconds(-1), _user);

        Assert.True(token.IsExpired());
        Assert.False(token.IsValid());
    }

    [Fact]
    public void MarkUsed_InvalidaOToken()
    {
        var token = new PasswordResetToken("hash", DateTime.UtcNow.AddMinutes(30), _user);

        token.MarkUsed();

        Assert.True(token.IsUsed());
        Assert.NotNull(token.UsedAt);
        Assert.False(token.IsValid());
    }
}
