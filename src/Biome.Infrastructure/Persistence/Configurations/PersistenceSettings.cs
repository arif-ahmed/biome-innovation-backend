namespace Biome.Infrastructure.Persistence.Configurations;

public class PersistenceSettings
{
    public const string SectionName = "Persistence";

    public string Provider { get; set; } = "InMemory";
    public LocalStackSettings LocalStack { get; set; } = new();
}

public class LocalStackSettings
{
    public bool Enabled { get; set; } = false;
    public bool EnsureTablesCreated { get; set; } = false;
    public string ServiceUrl { get; set; } = "http://localhost:4566";
    public string Region { get; set; } = "us-east-1";
}
