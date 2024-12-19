using System.Collections.Immutable;
using System.Diagnostics;
using Common;
using Serilog;

ILogger logger = new LoggerSetup().Logger;
Dictionary<(string design, int completedLength), long> matchCache = [];

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");

SolvePart2("Input/example.txt");
SolvePart2("Input/input.txt");

void SolvePart1(string filepath)
{
    var (patterns, designs) = ParseInput(filepath);

    var possibleCount = 0;
    foreach (var design in designs)
    {
        var patternsUsed = new List<string>();
        if (FindMatch(patterns, design, completedLength: 0, patternsUsed))
            possibleCount++;

        logger.Debug($"{design}: {string.Join(", ", patternsUsed)}");
    }

    logger.Information($"[Part1:{filepath}] Possible designs: {possibleCount}");
}

void SolvePart2(string filepath)
{
    var (patterns, designs) = ParseInput(filepath);
    matchCache.Clear();

    var sw = Stopwatch.StartNew();
    long possibleCount = 0;
    foreach (var design in designs)
    {
        var count = FindMatches(patterns, design, completedLength: 0);
        possibleCount += count;

        logger.Debug($"{design}: {count}");
    }
    sw.Stop();

    logger.Information($"[Part2:{filepath}] Possible arrangements: {possibleCount}  {sw.Elapsed.TotalMilliseconds}ms");
}

bool FindMatch(ImmutableSortedDictionary<int, HashSet<string>> patterns, string design, int completedLength, List<string> patternsUsed)
{
    logger.Verbose($"Called FindMatch: {design}, {completedLength}, Patterns used: ({string.Join(", ", patternsUsed)})");
    var foundMatch = false;

    foreach (var length in patterns.Keys)
    {
        if (length > design.Length - completedLength)
            continue;

        var region = design[completedLength..(completedLength + length)];

        if (patterns[length].Contains(region))
        {
            if (FindMatch(patterns, design, completedLength + length, patternsUsed))
            {
                patternsUsed.Add(region);
                completedLength += length;
                foundMatch = true;
                break;
            }
        }

        if (foundMatch)
            break;
    }

    if (design.Length == completedLength)
        return true;

    return foundMatch;
}


long FindMatches(ImmutableSortedDictionary<int, HashSet<string>> patterns, string design, int completedLength)
{
    logger.Verbose($"Called FindMatches: {design}, {completedLength}");

    if (design.Length == completedLength)
        return 1;

    if (matchCache.TryGetValue((design, completedLength), out var cachedFound))
        return cachedFound;

    long totalFound = 0;

    var start = Math.Min(design.Length - completedLength, patterns.Keys.First());
    for (int length = start; length >= 1; length--)
    {
        var region = design[completedLength..(completedLength + length)];

        if (patterns[length].Contains(region))
        {
            logger.Verbose(design[..(completedLength + length)]);
            var found = FindMatches(patterns, design, completedLength + length);

            matchCache[(design, completedLength + length)] = found;

            if (found > 0)
            {
                totalFound += found;
            }
        }
    }

    return totalFound;
}


Input ParseInput(string filepath)
{
    var content = File.ReadAllLines(filepath);

    var patterns = content[0]
        .Split(", ")
        .GroupBy(s => s.Length)
        .ToImmutableSortedDictionary(
            g => g.Key,
            g => g.ToHashSet(),
            Comparer<int>.Create((a, b) => b.CompareTo(a)));

    var designs = new List<string>();

    foreach (var design in content.Skip(2))
    {
        designs.Add(design);
    }

    return new Input(patterns, designs);
}
record Input(ImmutableSortedDictionary<int, HashSet<string>> Patterns, List<string> Designs);
