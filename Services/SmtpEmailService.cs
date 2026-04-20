using System.Net;
using System.Net.Mail;
using BetRoyale.API.Configurations;
using BetRoyale.API.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace BetRoyale.API.Services;

public class SmtpEmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailOptions> emailOptions, ILogger<SmtpEmailService> logger)
    {
        _emailOptions = emailOptions.Value;
        _logger = logger;
    }

    public async Task SendArticlePublishedNotificationAsync(
        IReadOnlyList<string> recipientEmails,
        string analystUsername,
        string articleTitle,
        string articleContent,
        CancellationToken cancellationToken = default)
    {
        if (!_emailOptions.Enabled)
        {
            _logger.LogInformation("Email notifications are disabled. Skipping article notification emails.");
            return;
        }

        if (!IsConfigured())
        {
            _logger.LogWarning("Email notifications are enabled but SMTP configuration is incomplete.");
            return;
        }

        var normalizedRecipients = recipientEmails
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .Select(email => email.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedRecipients.Count == 0)
        {
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(_emailOptions.FromEmail, _emailOptions.FromName),
            Subject = $"New article from {analystUsername}",
            Body = BuildBody(analystUsername, articleTitle, articleContent),
            IsBodyHtml = false
        };

        foreach (var recipientEmail in normalizedRecipients)
        {
            message.Bcc.Add(recipientEmail);
        }

        using var smtpClient = new SmtpClient(_emailOptions.Host, _emailOptions.Port)
        {
            EnableSsl = _emailOptions.UseSsl
        };

        if (!string.IsNullOrWhiteSpace(_emailOptions.Username))
        {
            smtpClient.Credentials = new NetworkCredential(_emailOptions.Username, _emailOptions.Password);
        }

        cancellationToken.ThrowIfCancellationRequested();
        await smtpClient.SendMailAsync(message);
    }

    private bool IsConfigured()
    {
        return !string.IsNullOrWhiteSpace(_emailOptions.Host) &&
               _emailOptions.Port > 0 &&
               !string.IsNullOrWhiteSpace(_emailOptions.FromEmail);
    }

    private static string BuildBody(string analystUsername, string articleTitle, string articleContent)
    {
        return
            $"A new article was published by {analystUsername}.{Environment.NewLine}{Environment.NewLine}" +
            $"Title: {articleTitle}{Environment.NewLine}{Environment.NewLine}" +
            $"Content:{Environment.NewLine}{articleContent}{Environment.NewLine}{Environment.NewLine}" +
            "You are receiving this notification because you are subscribed to this analyst.";
    }
}
