namespace IGeekFan.FreeKit.Extras.Extensions;

/// <summary>
/// IEnumerable 扩展
/// </summary>
public static class EnumerableExtensions
{
    public static IEnumerable<(T item, int index)> LoopIndex<T>(this IEnumerable<T> self) =>
        self.Select((item, index) => (item, index));
}