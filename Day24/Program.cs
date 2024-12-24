using System.Text.RegularExpressions;

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");

void SolvePart1(string filepath)
{
    var (inputs, connections) = ParseInput(filepath);

    var current = connections.First
        ?? throw new Exception("Input is missing connections");

    while (connections.Count > 0)
    {
        var next = current?.Next ?? connections.First;

        var connection = current.Value;

        if (!inputs.TryGetValue(connection.In1, out var in1) || !inputs.TryGetValue(connection.In2, out var in2))
        {
            current = next;
            continue;
        }

        inputs[connection.Out] = connection.Operation switch
        {
            Operation.AND => in1 & in2,
            Operation.XOR => in1 ^ in2,
            Operation.OR => in1 | in2,
            _ => throw new NotImplementedException("Unsupported operation")
        };

        var toRemove = current;
        current = next;
        connections.Remove(toRemove);
    }

    var result = inputs
        .Where(i => i.Key.StartsWith('z'))
        .OrderBy(i => i.Key)
        .Select((bit, index) => bit.Value ? 1L << index : 0)
        .Sum();

    Console.WriteLine($"[Part1:{filepath}] {result}");
}

Input ParseInput(string filepath)
{
    var content = File.ReadAllText(filepath)
        .Split(["\n\n", "\r\n\r\n"], StringSplitOptions.RemoveEmptyEntries);

    var inputLines = content[0]
        .Split(["\n", "\r\n"], StringSplitOptions.RemoveEmptyEntries);

    var inputs = new Dictionary<string, bool>();
    foreach (var line in inputLines)
    {
        var parts = line.Split(": ", 2);

        var id = parts[0];
        var value = parts[1] == "1";

        inputs[id] = value;
    }

    var connectionLines = content[1]
        .Split(["\n", "\r\n"], StringSplitOptions.RemoveEmptyEntries);

    var connections = new LinkedList<Connection>();
    var connectionRegex = new Regex(@"(\w+) (\w+) (\w+) -> (\w+)");
    foreach (var line in connectionLines)
    {
        var matches = connectionRegex.Match(line);

        connections.AddLast(new Connection
        (
            In1: matches.Groups[1].Value,
            Operation: matches.Groups[2].Value switch
            {
                "AND" => Operation.AND,
                "OR" => Operation.OR,
                "XOR" => Operation.XOR,
                _ => throw new Exception($"Invalid operation in input: '{line}'")
            },
            In2: matches.Groups[3].Value,
            Out: matches.Groups[4].Value
        ));
    }

    return new Input(inputs, connections);
}

record Input(Dictionary<string, bool> InputValues, LinkedList<Connection> Connections);
record Connection(string In1, string In2, string Out, Operation Operation);

enum Operation
{
    AND,
    XOR,
    OR,
}