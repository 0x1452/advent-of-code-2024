
using System.Diagnostics;

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");

SolvePart2("Input/example2.txt");
SolvePart2("Input/input.txt");

void SolvePart1(string filepath)
{
    var numbers = File.ReadAllLines(filepath).Select(long.Parse);

    long sum = 0;
    foreach (var number in numbers)
    {
        var number2000 = CalculateNthNumber(number, 2000);
        sum += number2000;
    }

    Console.WriteLine($"[Part1:{filepath}] {sum}");
}

void SolvePart2(string filepath)
{
    var startNumbers = File.ReadAllLines(filepath).Select(long.Parse);

    var sw = Stopwatch.StartNew();

    var secretNumberList = new List<List<long>>();
    foreach (var number in startNumbers)
    {
        var numbers = CalculateNNumbers(number, 2000);

        secretNumberList.Add(numbers);
    }

    var allSequences = new List<Dictionary<Sequence, long>>();
    foreach (var list in secretNumberList)
    {
        allSequences.Add(GetAllSequences(list));
    }
    
    var sequenceTotalCosts = new Dictionary<Sequence, long>();
    foreach (var listSequences in allSequences)
    {
        foreach (var (sequence, cost) in listSequences)
        {
            sequenceTotalCosts[sequence] = sequenceTotalCosts.GetValueOrDefault(sequence, 0) + cost;
        }
    }

    var maxPrice = sequenceTotalCosts.Values.Max();

    sw.Stop();

    Console.WriteLine($"[Part2:{filepath}] {maxPrice} {sw.Elapsed.TotalMilliseconds}ms");
}

Dictionary<Sequence, long> GetAllSequences(List<long> secretNumbers)
{
    var sequences = new Dictionary<Sequence, long>();
    var diffs = secretNumbers.Zip(secretNumbers.Skip(1), (a, b) => (b % 10) - (a % 10)).ToList();

    for (int i = 0; i < diffs.Count - 4; i++)
    {
        var sequence = new Sequence(diffs[i], diffs[i + 1], diffs[i + 2], diffs[i + 3]);
        var price = secretNumbers[i + 4] % 10;

        sequences.TryAdd(sequence, price);
    }

    return sequences;
}

long CalculateNthNumber(long number, int n)
{
    for (int i = 0; i < n; i++)
    {
        number ^= number * 64;
        number %= 16777216;

        number ^= number / 32;
        number %= 16777216;

        number ^= number * 2048;
        number %= 16777216;
    }
    return number;
}

List<long> CalculateNNumbers(long number, int n)
{
    var numbers = new List<long>();
    numbers.Add(number);

    for (int i = 0; i < n; i++)
    {
        number ^= number * 64;
        number %= 16777216;

        number ^= number / 32;
        number %= 16777216;

        number ^= number * 2048;
        number %= 16777216;

        numbers.Add(number);
    }

    return numbers;
}

record Sequence(long A, long B, long C, long D);
