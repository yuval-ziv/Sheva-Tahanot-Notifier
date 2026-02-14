using ShevaTahanotNotifier.Database.Entities;

namespace ShevaTahanotNotifier.Services;

public interface IBridgeStatusService
{
    Task<BridgeStatus> GetLastBridgeStatusAsync(bool skipCache = false, bool isManualRefresh = false, CancellationToken cancellationToken = default);
}