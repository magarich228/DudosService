using MailKit.Net.Smtp;
using MimeKit;
using System.ComponentModel.DataAnnotations;

namespace MagarichEmailService.Core
{
    public class EmailSender : IEmailSender
    {
        public string Email { get; private set; }
        private string Password { get; set; }

        public string SmtpHost { get; private set; }
        public int SmtpPort { get; private set; }
        public bool SmtpUseSsl { get; private set; }

        private readonly ILogger<EmailSender> _logger;

        public EmailSender(EmailCredentials credentials,
            SmtpOptions smtpOptions,
            ILoggerFactory logger)
        {
            this._logger = logger.CreateLogger<EmailSender>();

            ValidateParams(credentials);

            this.Email = credentials.Email;
            this.Password = credentials.Password;

            ValidateParams(smtpOptions);

            this.SmtpHost = smtpOptions.Host;
            this.SmtpPort = smtpOptions.Port;
            this.SmtpUseSsl = smtpOptions.UseSsl;
        }

        public async Task SendAsync(MimeMessage message) 
        {
            ValidMessage(message.To.Select(address => address.Name), message.TextBody);

            await RunSmtp(message); 
        }

        public async Task SendAsync(IEnumerable<string> recipients, string messText,
            string? html = null, string? subject = null, string? sender = null)
        {
            var message = BuildMessage(recipients, messText, html, subject, sender);
            ValidMessage(recipients, message.TextBody);

            await RunSmtp(message);
        }

        private async Task RunSmtp(MimeMessage message)
        {
            using SmtpClient smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(this.SmtpHost, this.SmtpPort, this.SmtpUseSsl);
            await smtpClient.AuthenticateAsync(this.Email, this.Password);

            await smtpClient.SendAsync(message);
            _logger.LogInformation($"Message {message.Subject} from {message.From.First().Name}" +
                $" to {message.To.Count} recipients was sent.");

            await smtpClient.DisconnectAsync(true);
        }

        public MimeMessage BuildMessage(IEnumerable<string> recipients, string messText,
            string? html, string? subject, string? sender)
        {
            MimeMessage mimeMess = new MimeMessage();

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = messText;
            bodyBuilder.HtmlBody = html;

            mimeMess.Body = bodyBuilder.ToMessageBody();
            mimeMess.Subject = subject ?? "Без темы";

            InternetAddressList internetAddresses = new InternetAddressList();
            foreach (var recipient in recipients)
            {
                internetAddresses.Add(new MailboxAddress($"recipient{Guid.NewGuid()}", recipient));
            }

            mimeMess.From.Add(new MailboxAddress(sender ?? "MagarichSenderService", Email));
            mimeMess.To.AddRange(internetAddresses);

            return mimeMess;
        }

        private void ValidateParams(object instance)
        {
            var validContext = new ValidationContext(instance);

            try
            {
                Validator.ValidateObject(instance, validContext);
            }
            catch (ValidationException)
            {
                throw new ArgumentException($"{nameof(instance)} in EmailSender contructor is not valid.");
            }
        }

        private void ValidMessage(IEnumerable<string> recipients, string messText)
        {
            if (recipients is null || recipients.Count() == 0)
                throw new ArgumentNullException(nameof(recipients));

            if (string.IsNullOrEmpty(messText))
                throw new ArgumentNullException(nameof(messText));
        }
    }
}
