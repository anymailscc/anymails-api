using Autofac;
using Autofac.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;

namespace AnyMails.Api.Extensions;

public static class HostExtensions
{
    public static IHostBuilder UseAutofacProviderFactory<TModule>(this IHostBuilder host)
    {
        host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(e =>
            {
                e.RegisterAssemblyModules(typeof(TModule).Assembly);
            });
        return host;
    }

    public static IHostBuilder UseCustomAppConfigurations(this IHostBuilder host)
    {
        host.ConfigureAppConfiguration((context, config) =>
        {
            var environment = context.HostingEnvironment.EnvironmentName;
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
            config.AddEnvironmentVariables();

            // These are optional configuration files only apply to local
            // DO NOT COMMIT THESE FILES!
            // These files should already be in .gitignore file
            config.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
            config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        });
        return host;
    }

    public static IHostBuilder UseCustomSerilog(this IHostBuilder host)
    {
        host.UseSerilog((context, services, config) =>
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
        return host;
    }
}
