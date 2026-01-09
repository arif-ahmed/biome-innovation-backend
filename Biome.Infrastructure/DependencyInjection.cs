namespace Biome.Infrastructure;

using Biome.SharedKernel.Abstractions;
using Biome.Domain.Users;
using Biome.Infrastructure.Authentication;
using Biome.Infrastructure.Persistence.Repositories;
using Biome.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<Biome.Domain.Roles.IRoleRepository, Biome.Infrastructure.Persistence.Repositories.InMemoryRoleRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IEmailService, Biome.Infrastructure.Services.Email.MockEmailService>();
        services.AddSingleton<ITwoFactorService, Biome.Infrastructure.Services.Authentication.MockTwoFactorService>();
        services.AddSingleton<IShippingService, Biome.Infrastructure.Services.Shipping.MockShippingService>();
        services.AddSingleton<Biome.Domain.Shipping.IShipmentRepository, Biome.Infrastructure.Persistence.Repositories.InMemoryShipmentRepository>();
        services.AddSingleton<Biome.Domain.Orders.IOrderRepository, Biome.Infrastructure.Persistence.Repositories.InMemoryOrderRepository>();
        services.AddSingleton<Biome.Domain.Payments.IPaymentRepository, Biome.Infrastructure.Persistence.Repositories.InMemoryPaymentRepository>();
        services.AddSingleton<Biome.Domain.Pets.IPetRepository, Biome.Infrastructure.Persistence.Repositories.InMemoryPetRepository>();
        services.AddSingleton<Biome.Application.Common.Interfaces.IPaymentGateway, Biome.Infrastructure.Services.Payments.MockPaymentGateway>();
        services.AddSingleton<Biome.Domain.Lab.ILabTestRepository, Biome.Infrastructure.Persistence.Repositories.InMemoryLabTestRepository>();
        services.AddSingleton<Biome.Domain.Reports.IHealthReportRepository, Biome.Infrastructure.Persistence.Repositories.InMemoryHealthReportRepository>();
        services.AddSingleton<Biome.Domain.Notifications.INotificationRepository, Biome.Infrastructure.Persistence.Repositories.InMemoryNotificationRepository>();
        services.AddSingleton<Biome.Domain.Support.ITicketRepository, Biome.Infrastructure.Persistence.Repositories.InMemoryTicketRepository>();

        services.AddAuthentication(defaultScheme: "Bearer")
            .AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Users:Read", policy => policy.RequireClaim("permission", "Users:Read"));
            options.AddPolicy("Users:Create", policy => policy.RequireClaim("permission", "Users:Create"));
            options.AddPolicy("Users:ReadSelf", policy => policy.RequireClaim("permission", "Users:ReadSelf"));
            options.AddPolicy("Roles:Create", policy => policy.RequireClaim("permission", "Roles:Create"));
        });

        return services;
    }
}
