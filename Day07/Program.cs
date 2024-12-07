using Common;
using Serilog;

ILogger logger = new LoggerSetup().Logger;

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");
SolvePart2("Input/example.txt");
SolvePart2("Input/input.txt");

void SolvePart1(string filepath)
{
    var content = File.ReadAllLines(filepath);
    var equations = new List<IncompleteEquation>();
    long sum = 0;
    foreach (var line in content)
    {
        var parts = line.Split(": ", 2);
        var result = long.Parse(parts[0]);
        var values = parts[1].Split().Select(long.Parse).ToList();

        var equation = new IncompleteEquation(result, values);
        var isPossible = IsPossible(equation);

        logger.Debug($"{isPossible}\t{line}");

        if (isPossible)
            sum += equation.Result;
    }

    logger.Information("[Part1:{File}] {Result}", filepath, sum);
}

void SolvePart2(string filepath)
{
    var content = File.ReadAllLines(filepath);
    var equations = new List<IncompleteEquation>();
    long sum = 0;
    foreach (var line in content)
    {
        var parts = line.Split(": ", 2);
        var result = long.Parse(parts[0]);
        var values = parts[1].Split().Select(long.Parse).ToList();

        var equation = new IncompleteEquation(result, values);
        var isPossible = IsPossible(equation, enableConcatenation: true);

        logger.Debug($"{isPossible}\t{line}");

        if (isPossible)
            sum += equation.Result;
    }

    logger.Information("[Part2:{File}] {Result}", filepath, sum);
}

bool IsPossible(IncompleteEquation equation, bool enableConcatenation = false)
{
    if (equation.Values.Count == 0)
        return false;

    if (equation.Values.Count == 1)
        return equation.Result == equation.Values[0];

    var addition = equation.Values[0] + equation.Values[1];
    var additionEquation = equation with { Values = [addition, .. equation.Values[2..]] };
    if (IsPossible(additionEquation, enableConcatenation))
        return true;

    var multiplication = equation.Values[0] * equation.Values[1];
    var multiplicationEquation = equation with { Values = [multiplication, .. equation.Values[2..]] };
    if (IsPossible(multiplicationEquation, enableConcatenation))
        return true;

    if (enableConcatenation)
    {
        var concatenation = long.Parse($"{equation.Values[0]}{equation.Values[1]}");
        var concatenationEquation = equation with { Values = [concatenation, .. equation.Values[2..]] };
        if (IsPossible(concatenationEquation, enableConcatenation))
            return true;
    }

    return false;
}

record IncompleteEquation(long Result, List<long> Values);
