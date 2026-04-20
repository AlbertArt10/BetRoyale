namespace BetRoyale.API.Services.Interfaces;

public interface IEmailService
{
    Task SendArticlePublishedNotificationAsync(
        IReadOnlyList<string> recipientEmails,
        string analystUsername,
        string articleTitle,
        string articleContent,
        CancellationToken cancellationToken = default);
}
