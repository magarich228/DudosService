using System.ComponentModel.DataAnnotations;

namespace MagarichEmailService.Core
{
    public class SmtpOptions
    {
        [Required]
        public string Host { get; set; } = string.Empty;

        [Required, Range(0, 10000)]
        public int Port { get; set; } = 0;

        public bool UseSsl { get; set; } = true;
    }
}
