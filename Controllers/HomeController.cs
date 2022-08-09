using MagarichEmailService.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MagarichEmailService.Controllers
{
    public class HomeController : Controller
    {
        readonly EmailMessageService _emailService;
        readonly ILogger<HomeController> _logger;

        public HomeController(EmailMessageService sender, ILogger<HomeController> logger)
        {
            _emailService = sender;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] MailingModel model) //bool randomSubject
        {
            if (!ModelState.IsValid || !model.ValidateRecipients())
            {
                _logger.LogInformation($"Error count: {ModelState.ErrorCount} {ModelState.GetFieldValidationState("Subject")} {ModelState.GetFieldValidationState("Message")}");
                return View(model);
            }

            _logger.LogInformation("Model is valid.");

            await _emailService.DudosAsync(model.Repeats, model.Recipients ?? 
                throw new ApplicationException(nameof(model.Recipients)), model.Message, 
                subject: model.Subject, randomSubject: true);

            return View("Success");
        }

        public class MailingModel
        {
            [Required, Range(1, 100, ErrorMessage = "range 1 to 100.")]
            public int Repeats { get; set; } = 1;

            [Required,
                MinLength(1, ErrorMessage = "there must be at least one client!"),
                MaxLength(10, ErrorMessage = "10 is max num of clients!")]
            public IEnumerable<string>? Recipients { get; set; }

            public string Message { get; set; } = string.Empty;
            public string? Subject { get; set; }

            public bool ValidateRecipients()
            {
                if (Recipients is null)
                    throw new ArgumentNullException(nameof(Recipients));

                var attrs = new List<ValidationAttribute>()
                {
                    new EmailAddressAttribute()
                };
                List<ValidationResult> results = new List<ValidationResult>();

                try
                {
                    foreach (var recipient in Recipients)
                    {
                        var validContext = new ValidationContext(recipient);
                        Validator.TryValidateValue(recipient, validContext, results, attrs);
                    }
                }
                catch (ValidationException)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
