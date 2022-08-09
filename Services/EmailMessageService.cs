using MagarichEmailService.Core;

namespace MagarichEmailService.Services
{
    public class EmailMessageService
    {
        private readonly IEmailSender _emailSender;

        public EmailMessageService(IEmailSender emailSender)
        {
            this._emailSender = emailSender;
        }

        public async Task SendAsync(IEnumerable<string> recipients, string messText,
            string? html = null, string? subject = null, string? sender = null) =>
            await _emailSender.SendAsync(recipients, messText, html, subject, sender);

        public async Task DudosAsync(int repeats, IEnumerable<string> recipients, 
            string messText, string? html = null, string? subject = null, string? sender = null, bool randomSubject = false)
        {
            for (int i = 0; i < repeats; i++)
                await _emailSender.SendAsync(recipients, messText, html, !randomSubject? subject : $"{i}", sender);
        }
    }
}
