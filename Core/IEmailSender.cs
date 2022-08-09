using MimeKit;

namespace MagarichEmailService.Core
{
    public interface IEmailSender
    {
        string Email { get; }

        string SmtpHost { get; }
        int SmtpPort { get; }
        bool SmtpUseSsl { get; }

        Task SendAsync(MimeMessage message);
        Task SendAsync(IEnumerable<string> recipients, 
            string messText,
            string? html = null,
            string? subject = null, 
            string? sender = null);

        MimeMessage BuildMessage(IEnumerable<string> recipients,
            string messText,
            string? html,
            string? subject,
            string? sender);
    }
}
