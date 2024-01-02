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
    .AddOutputCache(opt =>
    {
        opt.DefaultExpirationTimeSpan = TimeSpan.FromMinutes(1);
    })
    .AddStackExchangeRedisOutputCache(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("Redis");
        if(string.IsNullOrEmpty(connectionString)) return;

        options.InstanceName = Assembly.GetExecutingAssembly().GetName().Name;
        options.Configuration = connectionString;
    })
    .AddInfrastructure()
    .AddApplication()
    .AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseServiceHealthChecks();

app.UseOutputCache();

app.MapReleaseEndpoint();

app.MapGet("/", (ILogger logger) =>
    {
        logger.Information("Hello world");
        return "Hello World!";
    })
    .CacheOutput(x => x.Expire(TimeSpan.FromSeconds(30)));

app.Run();
