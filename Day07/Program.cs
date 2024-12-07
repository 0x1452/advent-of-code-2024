using Common;
using Serilog;
using System.Diagnostics;

ILogger logger = new LoggerSetup().Logger;

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");
SolvePart2("Input/example.txt");
SolvePart2("Input/input.txt");

List<IncompleteEquation> ParseInput(string filepath)
{
    var content = File.ReadAllLines(filepath);
    var equations = new List<IncompleteEquation>();

    foreach (var line in content)
    {
        var parts = line.Split(": ", 2);
        var result = long.Parse(parts[0]);
        var values = parts[1].Split().Select(long.Parse).ToList();

        equations.Add(new IncompleteEquation(result, values));
    }

    return equations;
}

void SolvePart1(string filepath)
{
    var equations = ParseInput(filepath);

    var sw = Stopwatch.StartNew();
    long sum = equations
        .Where(e => IsPossible(e))
        .Sum(e => e.Result);
    sw.Stop();

    logger.Information("[Part1:{File}] {Result}  {ElapsedMs}ms", filepath, sum, sw.Elapsed.TotalMilliseconds);
}

void SolvePart2(string filepath)
{
    var equations = ParseInput(filepath);

    var sw = Stopwatch.StartNew();
    long sum = equations
        .Where(e => IsPossible(e, enableConcatenation: true))
        .Sum(e => e.Result);
    sw.Stop();

    logger.Information("[Part2:{File}] {Result}  {ElapsedMs}ms", filepath, sum, sw.Elapsed.TotalMilliseconds);
}

bool IsPossible(IncompleteEquation equation, int index = 0, bool enableConcatenation = false)
{
    if (equation.Values.Count == 0)
        return false;

    if (index == equation.Values.Count - 1)
        return equation.Result == equation.Values[index];

    if (equation.Values[index] > equation.Result)
        return false;

    var originalNextValue = equation.Values[index + 1];

    equation.Values[index + 1] = equation.Values[index] * equation.Values[index + 1];
    if (IsPossible(equation, index + 1, enableConcatenation))
        return true;
    equation.Values[index + 1] = originalNextValue;

    equation.Values[index + 1] = equation.Values[index] + equation.Values[index + 1];
    if (IsPossible(equation, index + 1, enableConcatenation))
        return true;
    equation.Values[index + 1] = originalNextValue;

    if (enableConcatenation)
    {
        var concatenated = long.Parse($"{equation.Values[index]}{equation.Values[index + 1]}");
        equation.Values[index + 1] = concatenated;
        if (IsPossible(equation, index + 1, enableConcatenation))
            return true;
        equation.Values[index + 1] = originalNextValue;
    }

    return false;
}

record IncompleteEquation(long Result, List<long> Values);
