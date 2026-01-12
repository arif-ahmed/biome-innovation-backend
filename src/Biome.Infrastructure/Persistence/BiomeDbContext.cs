using Biome.Domain.Support;
using Biome.Domain.Users;
using Biome.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Biome.Infrastructure.Persistence;

public class BiomeDbContext : DbContext, IUnitOfWork
{
    public BiomeDbContext(DbContextOptions<BiomeDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BiomeDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}
