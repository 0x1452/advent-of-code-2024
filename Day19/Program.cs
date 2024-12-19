// See https://aka.ms/new-console-template for more information
using System.Collections.Immutable;
using Common;
using Serilog;

ILogger logger = new LoggerSetup().Logger;
SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");


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


bool FindMatch(ImmutableSortedDictionary<int, List<string>> patterns, string design, int completedLength, List<string> patternsUsed)
{
    logger.Verbose($"Called FindMatch: {design}, {completedLength}, Patterns used: ({string.Join(", ", patternsUsed)})");
    var foundMatch = false;

    foreach (var length in patterns.Keys)
    {
        if (length > design.Length - completedLength)
            continue;

        var region = design[completedLength..(completedLength + length)];

        foreach (var pattern in patterns[length])
        {
            if (region == pattern)
            {
                if (FindMatch(patterns, design, completedLength + length, patternsUsed))
                {
                    patternsUsed.Add(pattern);
                    completedLength += length;
                    foundMatch = true;
                    break;
                }
            }
        }

        if (foundMatch)
            break;
    }

    if (design.Length == completedLength)
        return true;

    return foundMatch;
}

Input ParseInput(string filepath)
{
    var content = File.ReadAllLines(filepath);

    var patterns = content[0]
        .Split(", ")
        .GroupBy(s => s.Length)
        .ToImmutableSortedDictionary(
            g => g.Key,
            g => g.ToList(),
            Comparer<int>.Create((a, b) => b.CompareTo(a)));

    var designs = new List<string>();

    foreach (var design in content.Skip(2))
    {
        designs.Add(design);
    }

    return new Input(patterns, designs);
}
record Input(ImmutableSortedDictionary<int, List<string>> Patterns, List<string> Designs);
