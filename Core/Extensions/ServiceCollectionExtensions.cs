namespace MagarichEmailService.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppCore(this IServiceCollection services, IConfiguration configuration)
        {
            EmailCredentials emailCredentials = new();
            configuration.GetSection("EmailSender").Bind("Credentials", emailCredentials);

            SmtpOptions smtpOptions = new();
            configuration.GetSection("EmailSender").Bind("SmtpOptions", smtpOptions);

            services.AddScoped<IEmailSender, EmailSender>((serviceProvider) =>
            {
                var logger = serviceProvider.GetService<ILoggerFactory>() ?? throw new ApplicationException();

                return new EmailSender(emailCredentials, smtpOptions, logger);
            });

            return services;
        }
    }
}
