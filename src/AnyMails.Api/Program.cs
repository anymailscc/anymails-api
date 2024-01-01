using AnyMails.Api;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);
builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(e => e.RegisterAssemblyModules(typeof(AutofacModule).Assembly))
    .UseSerilog((context, services, config) =>
    {
        var seqHostName = context.Configuration["Seq:HostName"];
        var seqApiKey = context.Configuration["Seq:ApiKey"];
        if(!string.IsNullOrWhiteSpace(seqHostName) && !string.IsNullOrWhiteSpace(seqApiKey))
        {
            config.WriteTo.Seq(seqHostName, apiKey: seqApiKey);
        }

        config
#if DEBUG
            .Enrich.WithProperty("Environment", "Local")
#else
            .Enrich.WithProperty("Environment", context.Configuration["ASPNETCORE_ENVIRONMENT"])
#endif
            .Enrich.WithCorrelationId()
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .Enrich.WithExceptionDetails()
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services);
    });

var app = builder.Build();

app.MapGet("/", (ILogger logger) =>
{
    logger.Information("Hello world");
    return "Hello World!";
});

app.Run();
