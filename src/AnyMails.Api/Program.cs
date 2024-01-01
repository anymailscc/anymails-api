using AnyMails.Api;
using AnyMails.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseAutofacProviderFactory<AutofacModule>()
    .UseCustomAppConfigurations()
    .UseCustomSerilog();

var app = builder.Build();

app.MapGet("/", (ILogger logger) =>
{
    logger.Information("Hello world");
    return "Hello World!";
});

app.Run();
