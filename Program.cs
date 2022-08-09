using MagarichEmailService;
using MagarichEmailService.Core.Extensions;
using MagarichEmailService.Services;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

configuration.AddJsonFile("ServicesConfiguration.json");

services.AddMvc();
services.AddAppCore(configuration);
services.AddAppServices(configuration);

var app = builder.Build();

app.UseStaticFiles();

app.MapDefaultControllerRoute();
app.Run();