using Microsoft.Extensions.DependencyInjection;

namespace AnyMails.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
