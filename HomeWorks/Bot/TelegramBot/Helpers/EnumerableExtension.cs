namespace Bot;

public static class EnumerableExtension
{
    public static IEnumerable<T> GetBatchByNumber<T>(this IEnumerable<T> enumerable, int batchSize, int batchNumber)
    {
        return enumerable.Skip(batchNumber * batchSize).Take(batchSize);
    }
}