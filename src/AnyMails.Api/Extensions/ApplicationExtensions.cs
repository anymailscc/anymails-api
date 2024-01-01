using Microsoft.AspNetCore.Mvc;

namespace AnyMails.Api.Extensions;

public static class ApplicationExtensions
{
    public static IApplicationBuilder UseServiceHealthChecks(this IApplicationBuilder app, string path = "/health")
    {
        app.UseHealthChecks(path);
        return app;
    }

    public static WebApplication MapReleaseEndpoint(this WebApplication app)
    {
        app.MapGet("release", ([FromServices] IConfiguration config) => Results.Ok(new
        {
            release = config["Release"],
            sha = config["Sha"],
#if DEBUG
            environment = "Local"
#else
            environment = config["ASPNETCORE_ENVIRONMENT"]
#endif
        }));
        return app;
    }
}
