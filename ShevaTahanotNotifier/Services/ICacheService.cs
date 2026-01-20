namespace ShevaTahanotNotifier.Services;

public interface ICacheService
{
    ValueTask<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, ValueTask<T>> factory, CancellationToken cancellationToken = default);
    public ValueTask SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);
}