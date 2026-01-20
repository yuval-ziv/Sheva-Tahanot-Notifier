using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories;

namespace ShevaTahanotNotifier.Services;

public class BridgeStatusService : IBridgeStatusService
{
    private const string BridgeStatusCacheKey = "/BridgeStatusService/CurrentBridgeStatus";

    private readonly ILogger<BridgeStatusService> _logger;
    private readonly ICacheService _cache;
    private readonly IBridgeStatusRepository _repository;
    private readonly IBridgeStatusFetcher _fetcher;

    public BridgeStatusService(ILogger<BridgeStatusService> logger, ICacheService cache, IBridgeStatusRepository repository, IBridgeStatusFetcher fetcher)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
        _fetcher = fetcher;
    }

    public async Task<BridgeStatus> GetLastBridgeStatusAsync(bool skipCache = false, CancellationToken cancellationToken = default)
    {
        bool isManualRefresh = skipCache;
        if (!skipCache)
        {
            _logger.LogDebug("Fetching last bridge status from cache");
            return await _cache.GetOrCreateAsync(BridgeStatusCacheKey, async cancel => await UpdateBridgeStatusAsync(isManualRefresh, cancel), cancellationToken: cancellationToken);
        }

        _logger.LogDebug("Fetching last bridge status from cache");
        BridgeStatus status = await UpdateBridgeStatusAsync(isManualRefresh, cancellationToken);
        _logger.LogDebug("Last bridge status updated from cache. Current status is open={Status}. Updating cache", status.IsOpen);
        await _cache.SetAsync(BridgeStatusCacheKey, status, cancellationToken);
        _logger.LogDebug("Updated bridge status in cache");
        return status;
    }

    public async Task<BridgeStatus> UpdateBridgeStatusAsync(bool isManualRefresh, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating bridge status");
        bool isBridgeOpen = await _fetcher.FetchBridgeStatusAsync(cancellationToken);
        var bridgeStatus = new BridgeStatus
        {
            IsOpen = isBridgeOpen,
            IsManualRefresh = isManualRefresh,
            LastUpdated = DateTimeOffset.UtcNow,
        };

        _logger.LogDebug("Creating new bridge status with open={IsOpen}", bridgeStatus.IsOpen);
        await _repository.CreateAsync(bridgeStatus, cancellationToken: cancellationToken);
        _logger.LogDebug("Created new bridge status");
        return bridgeStatus;
    }
}