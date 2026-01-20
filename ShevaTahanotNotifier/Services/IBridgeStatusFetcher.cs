namespace ShevaTahanotNotifier.Services;

public interface IBridgeStatusFetcher
{
    Task<bool> FetchBridgeStatusAsync(CancellationToken cancellationToken = default);
}