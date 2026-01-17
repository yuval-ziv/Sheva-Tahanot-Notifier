using ShevaTahanotNotifier.Database;

namespace ShevaTahanotNotifier.Configuration;

public class NotifierContextOptions
{
    public const string ConfigurationSectionName = "NotifierContext";
    public required DatabaseProviderType DatabaseProviderType { get; set; }
    public required string ConnectionString { get; set; }
}