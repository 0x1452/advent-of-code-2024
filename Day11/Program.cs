using System.Diagnostics;

Dictionary<long, (long? first, long second)> stoneCache = [];
Dictionary<(long? first, long second, int iterationsLeft), long> iterationCache = [];

Solve("Input/example.txt", 6);
Solve("Input/input.txt", 25);
Solve("Input/input.txt", 75);

void Solve(string filepath, int iterations)
{
    var stones = File.ReadAllText(filepath)
        .Split(" ", StringSplitOptions.RemoveEmptyEntries)
        .Select(long.Parse)
        .ToList();

    var sw = Stopwatch.StartNew();

    long count = 0;
    foreach (var stone in stones)
    {
        count += ProcessStones(null, stone, iterations);
    }

    sw.Stop();

    Console.WriteLine($"[{filepath}:{iterations}] {count} {sw.Elapsed.TotalMilliseconds}ms");
}

long ProcessStones(long? first, long second, int iterationsLeft)
{
    if (iterationsLeft == 0)
        return first.HasValue ? 2 : 1;

    if (iterationCache.TryGetValue((first, second, iterationsLeft), out var count))
        return count;

    long sum = 0;

    var (a, b) = ProcessStone(second);
    sum += ProcessStones(a, b, iterationsLeft - 1);

    if (first.HasValue)
    {
        (a, b) = ProcessStone(first.Value);
        sum += ProcessStones(a, b, iterationsLeft - 1);
    }

    iterationCache.TryAdd((first, second, iterationsLeft), sum);
    return sum;
}

(long? first, long second) ProcessStone(long stone)
{
    if (stoneCache.TryGetValue(stone, out var cachedResult))
    {
        return cachedResult;
    }

    int digitCount;
    if (stone == 0)
    {
        return (null, 1);
    }
    else if ((digitCount = CountDigits(stone)) % 2 == 0)
    {
        var divisor = (long)Math.Pow(10, digitCount / 2);
        var firstHalf = stone / divisor;
        var secondHalf = stone % divisor;

        return (firstHalf, secondHalf);
    }
    else
    {
        return (null, stone * 2024);
    }
}

int CountDigits(long value)
{
    int digits = 0;
    do
    {
        digits++;
        value /= 10;
    } while (value > 0);

    return digits;
}