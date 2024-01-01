using AnyMails.Api.Extensions;
using AnyMails.Application;
using AnyMails.Infrastructure;
using AutofacModule = AnyMails.Api.AutofacModule;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseAutofacProviderFactory<AutofacModule>()
    .UseCustomAppConfigurations()
    .UseCustomSerilog();

builder.Services
    .AddJsonOptions()
    .Configure<RouteOptions>(options => { options.LowercaseUrls = true; })
    .AddInfrastructure()
    .AddApplication()
    .AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseServiceHealthChecks();

app.MapReleaseEndpoint();

app.MapGet("/", (ILogger logger) =>
{
    logger.Information("Hello world");
    return "Hello World!";
});

app.Run();
