using System.Text.RegularExpressions;
using Day17;

var registerRegex = new Regex(@"Register [A|B|C]: (\d+)");

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");

void SolvePart1(string filepath)
{
    var computer = ParseInput(filepath);

    computer.Run();

    var result = string.Join(",", computer.Output);
    Console.WriteLine($"[Part1:{filepath}] {result}");
}

ChronospatialComputer ParseInput(string filepath)
{
    var lines = File.ReadAllLines(filepath);

    var a = int.Parse(registerRegex.Match(lines[0]).Groups[1].Value);
    var b = int.Parse(registerRegex.Match(lines[1]).Groups[1].Value);
    var c = int.Parse(registerRegex.Match(lines[2]).Groups[1].Value);

    var program = lines[4].Split()[1].Split(",").Select(byte.Parse).ToList();

    return new ChronospatialComputer(a, b, c, program);
}