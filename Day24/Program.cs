using System.Text.RegularExpressions;

Solve("Input/example.txt");
Solve("Input/input.txt");

Solve("Input/input_fixed.txt");


// I "manually" solved part 2:
// - Exported a Graphviz graph
// - First, I manually looked for x/y pairs that don't reach their `z` via two XOR jumps
// - Then, I started comparing the binary representation of the current `Z` and the expected `Z` -> the first bit where it starts differing shows you the first invalid "adder"
// -> I manually fixed the connections in the input, and checked where the Z differs again until I had the solution
void Solve(string filepath)
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

    SaveGraphviz(filepath, inputs);

    var resultX = inputs
        .Where(i => i.Key.StartsWith('x'))
        .OrderBy(i => i.Key)
        .Select((bit, index) => bit.Value ? 1L << index : 0)
        .Sum();

    var resultY = inputs
        .Where(i => i.Key.StartsWith('y'))
        .OrderBy(i => i.Key)
        .Select((bit, index) => bit.Value ? 1L << index : 0)
        .Sum();

    var resultZ = resultX + resultY;

    Console.WriteLine($"X:            {Convert.ToString(resultX, 2).PadLeft(46, '0')}");
    Console.WriteLine($"Y:            {Convert.ToString(resultY, 2).PadLeft(46, '0')}");
    Console.WriteLine($"Z (expected): {Convert.ToString(resultZ, 2).PadLeft(46, '0')}");
    Console.WriteLine($"Z (actual):   {Convert.ToString(result, 2).PadLeft(46, '0')}");

    Console.WriteLine($"[Part1:{filepath}] {result}");
}

void SaveGraphviz(string filepath, Dictionary<string, bool> inputs)
{
    var (_, connections) = ParseInput(filepath);

    var filename = Path.GetFileName(filepath) + ".graph.dot";
    using var writer = new StreamWriter(filename);

    writer.WriteLine("digraph Circuit {");
    writer.WriteLine("  rankdir=LR;");

    foreach (var input in inputs.Keys)
    {
        writer.WriteLine($"  \"{input}\" [shape=circle];");
    }

    foreach (var connection in connections)
    {
        writer.WriteLine($"  \"{connection.In1}\" -> \"{connection.Out}\" [label=\"{connection.Operation}\"];");
        writer.WriteLine($"  \"{connection.In2}\" -> \"{connection.Out}\" [label=\"{connection.Operation}\"];");
    }

    writer.WriteLine("}");
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