using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Biome.Application.Common.Interfaces;
using Biome.Domain.Lab;
using Biome.Domain.Notifications;
using Biome.Domain.Orders;
using Biome.Domain.Payments;
using Biome.Domain.Pets;
using Biome.Domain.Reports;
using Biome.Domain.Roles;
using Biome.Domain.Shipping;
using Biome.Domain.Support;
using Biome.Domain.Users;
using Biome.Infrastructure.Persistence.Configurations;
using Biome.Infrastructure.Persistence.Initialization;
using Biome.Infrastructure.Persistence.Repositories;
using Biome.Infrastructure.Persistence.Repositories.DynamoDb;
using Biome.Infrastructure.LocalStack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Biome.Infrastructure.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PersistenceSettings>(configuration.GetSection(PersistenceSettings.SectionName));
        var persistenceSettings = configuration.GetSection(PersistenceSettings.SectionName).Get<PersistenceSettings>();
        
        // Default to InMemory if not specified
        var provider = persistenceSettings?.Provider ?? "InMemory";

        switch (provider)
        {
            case "DynamoDb":
                services.AddDynamoDbPersistence(configuration);
                break;
            case "Postgres":
                services.AddPostgresPersistence(configuration);
                break;
            case "InMemory":
            default:
                services.AddInMemoryPersistence();
                break;
        }

        return services;
    }

    public static IServiceCollection AddInMemoryPersistence(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IRoleRepository, InMemoryRoleRepository>();
        services.AddSingleton<IShipmentRepository, InMemoryShipmentRepository>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
        services.AddSingleton<IPetRepository, InMemoryPetRepository>();
        services.AddSingleton<ILabTestRepository, InMemoryLabTestRepository>();
        services.AddSingleton<IHealthReportRepository, InMemoryHealthReportRepository>();
        services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();
        services.AddSingleton<ITicketRepository, InMemoryTicketRepository>();

        return services;
    }

    public static IServiceCollection AddDynamoDbPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // Add LocalStack configuration first
        services.AddLocalStackServices(configuration);

        // AWS Options are usually picked up automatically from appsettings "AWS" section 
        // if using AddDefaultAWSOptions, but here we can be explicit or rely on default.
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        
        // Add AmazonDynamoDB client if not already registered (e.g., by LocalStack)
        var dynamoDbDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IAmazonDynamoDB));
        if (dynamoDbDescriptor == null)
        {
            services.AddAWSService<IAmazonDynamoDB>();
        }

        services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

        // Register table initializer for development
        services.AddSingleton<DynamoDbTableInitializer>();

        // Register DynamoDB implementations
        services.AddSingleton<IUserRepository, DynamoDbUserRepository>();
        services.AddSingleton<ITicketRepository, DynamoDbTicketRepository>();
        
        // Fallbacks for not-yet-implemented DynamoDB repos
        services.AddSingleton<IRoleRepository, InMemoryRoleRepository>();
        services.AddSingleton<IShipmentRepository, InMemoryShipmentRepository>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
        services.AddSingleton<IPetRepository, InMemoryPetRepository>();
        services.AddSingleton<ILabTestRepository, InMemoryLabTestRepository>();
        services.AddSingleton<IHealthReportRepository, InMemoryHealthReportRepository>();
        services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();

        return services;
    }

    public static IServiceCollection AddPostgresPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BiomeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, Biome.Infrastructure.Persistence.Repositories.Postgres.PostgresUserRepository>();
        services.AddScoped<ITicketRepository, Biome.Infrastructure.Persistence.Repositories.Postgres.PostgresTicketRepository>();

        // Fallbacks for not-yet-implemented Postgres repos
        services.AddSingleton<IRoleRepository, InMemoryRoleRepository>();
        services.AddSingleton<IShipmentRepository, InMemoryShipmentRepository>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
        services.AddSingleton<IPetRepository, InMemoryPetRepository>();
        services.AddSingleton<ILabTestRepository, InMemoryLabTestRepository>();
        services.AddSingleton<IHealthReportRepository, InMemoryHealthReportRepository>();
        services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();

        return services;
    }
}
