using MagarichEmailService.Services;

namespace MagarichEmailService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<EmailMessageService>();

            return services;
        }
    }
}
