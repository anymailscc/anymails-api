using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnyMails.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        services.AddOptions<JsonSerializerOptions>()
            .Configure(options =>
            {
                options.PropertyNameCaseInsensitive = false;
                options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

                options.Converters.Add(new JsonStringEnumConverter());
            });

        return services;
    }
}
