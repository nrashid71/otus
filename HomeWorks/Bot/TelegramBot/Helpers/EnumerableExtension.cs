namespace Bot;

public static class EnumerableExtension
{
    public static IEnumerable<T> GetBatchByNumber<T>(this IEnumerable<T> enumerable, int batchSize, int batchNumber)
    {
        if (batchSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(batchSize));
        }

        if (batchNumber < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(batchNumber));
        }
        return enumerable.Skip(batchNumber * batchSize).Take(batchSize);
    }
}