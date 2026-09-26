using System.Net;

namespace Recipes.Application.Services.Auth;

internal static class PasswordResetEmails
{
    public const string ResetSubject = "ChefArchive - Redefinição de senha";
    public const string ChangedSubject = "ChefArchive - Sua senha foi alterada";

    public static string ResetBody(string userName, string resetLink, int expirationMinutes)
    {
        var name = WebUtility.HtmlEncode(userName);
        var link = WebUtility.HtmlEncode(resetLink);

        return $"""
                <p>Olá, {name}.</p>
                <p>Recebemos um pedido para redefinir a senha da sua conta no ChefArchive.</p>
                <p><a href="{link}">Clique aqui para escolher uma nova senha</a>.</p>
                <p>O link expira em {expirationMinutes} minutos e só pode ser usado uma vez.</p>
                <p>Se você não pediu a redefinição, ignore este email: sua senha continua a mesma.</p>
                """;
    }

    public static string ChangedBody(string userName)
    {
        var name = WebUtility.HtmlEncode(userName);

        return $"""
                <p>Olá, {name}.</p>
                <p>A senha da sua conta no ChefArchive acabou de ser alterada e todas as sessões abertas foram encerradas.</p>
                <p>Se não foi você, redefina sua senha imediatamente.</p>
                """;
    }
}
