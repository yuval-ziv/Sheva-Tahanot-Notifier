namespace ShevaTahanotNotifier.ExtensionMethods;

public static class EnumerableExtensions
{
    public static ICollection<T> ToCollection<T>(this IEnumerable<T>? enumerable)
    {
        return enumerable switch
        {
            null => Array.Empty<T>(),
            ICollection<T> collection => collection,
            _ => enumerable.ToArray()
        };
    }

    public static async Task ForEachAsync<T>(this IAsyncEnumerable<T> enumerable, Action<T> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(action);

        await foreach (T item in enumerable.WithCancellation(cancellationToken))
        {
            action(item);
        }
    }
}