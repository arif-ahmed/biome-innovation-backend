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
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = localStackSettings.ServiceUrl,
                AuthenticationRegion = localStackSettings.Region,
                UseHttp = true
            };
            
            services.AddSingleton(config);
            services.AddSingleton<IAmazonDynamoDB>(sp => new AmazonDynamoDBClient(config));
        }

        return services;
    }
}
