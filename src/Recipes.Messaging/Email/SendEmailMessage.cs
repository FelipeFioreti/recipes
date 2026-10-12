namespace Recipes.Messaging.Email;

public record SendEmailMessage(string MessageId, string To, string Subject, string HtmlBody);
