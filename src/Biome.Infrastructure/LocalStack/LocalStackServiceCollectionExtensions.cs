using Amazon.DynamoDBv2;
using Biome.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Biome.Infrastructure.LocalStack;

public static class LocalStackServiceCollectionExtensions
{
    public static IServiceCollection AddLocalStackServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var localStackSettings = configuration.GetSection("Persistence:LocalStack").Get<LocalStackSettings>();
        
        if (localStackSettings?.Enabled == true)
        {
            services.Configure<AmazonDynamoDBConfig>(config =>
            {
                config.ServiceURL = localStackSettings.ServiceUrl;
                config.AuthenticationRegion = localStackSettings.Region;
                // Disable SSL for LocalStack
                config.UseHttp = true;
            });
        }

        return services;
    }
}
