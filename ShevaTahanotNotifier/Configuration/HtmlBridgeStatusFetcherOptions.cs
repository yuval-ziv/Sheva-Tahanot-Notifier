namespace ShevaTahanotNotifier.Configuration;

public class HtmlBridgeStatusFetcherOptions
{
    public const string ConfigurationSectionName = "HtmlBridgeStatusFetcher";

    public required string HtmlUrl { get; set; }
    public required string StatusElementXpathSelector { get; set; }
}