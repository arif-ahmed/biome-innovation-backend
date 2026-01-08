namespace Biome.Infrastructure;

using Biome.Application.Common.Interfaces;
using Biome.Application.Common.Interfaces.Authentication;
using Biome.Domain.Users;
using Biome.Infrastructure.Authentication;
using Biome.Infrastructure.Persistence.Repositories;
using Biome.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IEmailService, EmailService>();

        return services;
    }
}
