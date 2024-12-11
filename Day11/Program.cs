using System.Diagnostics;

Dictionary<(long, int), long> iterationCache = [];

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


    var count = stones.Sum(s => ProcessStone(s, iterations));

    sw.Stop();

    Console.WriteLine($"[{filepath}:{iterations}] {count} {sw.Elapsed.TotalMilliseconds}ms");
}


long ProcessStone(long stone, int iterationsLeft)
{
    if (iterationsLeft == 0)
        return 1;

    if (iterationCache.TryGetValue((stone, iterationsLeft), out var count))
        return count;

    long result;
    int digitCount;
    if (stone == 0)
    {
        result = ProcessStone(1, iterationsLeft - 1);
    }
    else if ((digitCount = CountDigits(stone)) % 2 == 0)
    {
        var divisor = (long)Math.Pow(10, digitCount / 2);
        var firstHalf = stone / divisor;
        var secondHalf = stone % divisor;

        result = ProcessStone(firstHalf, iterationsLeft - 1) + ProcessStone(secondHalf, iterationsLeft - 1);
    }
    else
    {
        result = ProcessStone(stone * 2024, iterationsLeft - 1);
    }

    iterationCache.TryAdd((stone, iterationsLeft), result);
    return result;
}

int CountDigits(long value)
    => (int)Math.Log10(value) + 1;