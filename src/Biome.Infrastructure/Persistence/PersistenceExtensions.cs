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
using Biome.Infrastructure.Persistence.Repositories;
using Biome.Infrastructure.Persistence.Repositories.DynamoDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        // AWS Options are usually picked up automatically from appsettings "AWS" section 
        // if using AddDefaultAWSOptions, but here we can be explicit or rely on default.
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

        // Register DynamoDB implementations
        // Note: For now we only have TicketRepository, others might need stubs or fallback to InMemory for hybrid (but purely we should implement all)
        // To allow build to pass without implementing everything immediately:
        // We will use InMemory for the missing ones temporarily or we should warn the user.
        // For this task, I will map TicketRepository to DynamoDbTicketRepository
        // and others to InMemory to prevent crash/build error, BUT logically this is "Generic Persistence", so eventually all must be real.
        
        services.AddSingleton<ITicketRepository, DynamoDbTicketRepository>();
        
        // Fallbacks for not-yet-implemented DynamoDB repos
        services.AddSingleton<IUserRepository, InMemoryUserRepository>(); 
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
