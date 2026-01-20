using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using ShevaTahanotNotifier.Configuration;

namespace ShevaTahanotNotifier.Services;

public class HtmlBridgeStatusFetcher : IBridgeStatusFetcher
{
    private readonly ILogger<HtmlBridgeStatusFetcher> _logger;
    private readonly HtmlBridgeStatusFetcherOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;

    public HtmlBridgeStatusFetcher(ILogger<HtmlBridgeStatusFetcher> logger, IOptionsMonitor<HtmlBridgeStatusFetcherOptions> configurationMonitor, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _options = configurationMonitor.CurrentValue;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> FetchBridgeStatusAsync(CancellationToken cancellationToken = default)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient();
        string html = await httpClient.GetStringAsync(_options.HtmlUrl, cancellationToken);
        var document = new HtmlDocument();
        document.LoadHtml(html);

        HtmlNode bridgeStatusNode = document.DocumentNode.SelectSingleNode(_options.StatusElementXpathSelector);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract - SelectSingleNode can be null 
        if (bridgeStatusNode is null)
        {
            _logger.LogWarning("Unable to extract bridge status from HTML");
            return false;
        }

        string text = bridgeStatusNode.InnerText.Trim();
        _logger.LogDebug("Got bridge status node with text {Text}", text);
        return !text.Contains("סגור");
    }
}