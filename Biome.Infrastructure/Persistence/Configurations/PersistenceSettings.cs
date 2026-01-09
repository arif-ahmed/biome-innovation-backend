namespace Biome.Infrastructure.Persistence.Configurations;

public class PersistenceSettings
{
    public const string SectionName = "Persistence";

    public string Provider { get; set; } = "InMemory";
}
