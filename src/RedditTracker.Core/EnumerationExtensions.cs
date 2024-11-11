namespace RedditTracker.Core;

public static class EnumerationExtensions {
    public static IEnumerable<R> Enumerate<T, R>(
        this IEnumerable<T> target,
        Func<T, int, R> selector
    ) {
        var ct = 0;
        foreach (var i in target) {
            yield return selector(i, ct++);
        }
    }
}
