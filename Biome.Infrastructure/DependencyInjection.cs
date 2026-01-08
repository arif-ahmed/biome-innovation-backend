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
