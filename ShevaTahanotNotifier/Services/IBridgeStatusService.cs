using ShevaTahanotNotifier.Database.Entities;

namespace ShevaTahanotNotifier.Services;

public interface IBridgeStatusService
{
    Task<BridgeStatus> GetLastBridgeStatusAsync(bool skipCache = false, CancellationToken cancellationToken = default);
    Task<BridgeStatus> UpdateBridgeStatusAsync(bool isManualRefresh, CancellationToken cancellationToken = default);
}