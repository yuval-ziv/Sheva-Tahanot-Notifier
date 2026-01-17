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
}