using System.Reflection;
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
    .AddCustomOutputCache(options =>
    {
        options.InstanceName = Assembly.GetExecutingAssembly().FullName;
        options.Configuration = $"{builder.Configuration["Redis:Host"]}:{builder.Configuration["Redis:Port"]}";
    })
    .AddInfrastructure()
    .AddApplication()
    .AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseOutputCache();

app.UseServiceHealthChecks();

app.MapReleaseEndpoint();

app.MapGet("/", (ILogger logger) =>
{
    logger.Information("Hello world");
    return "Hello World!";
})
.CacheOutput(x => x.Expire(TimeSpan.FromSeconds(30)));

app.Run();
