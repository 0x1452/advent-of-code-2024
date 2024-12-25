
using Common;

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");


void SolvePart1(string filepath)
{
    var (locks, keys) = ParseInput(filepath);

    Dictionary<(int index, int height), HashSet<int>> lockHeights = GroupHeights(locks);
    Dictionary<(int index, int height), HashSet<int>> keyHeights = GroupHeights(keys);

    var matchesFound = 0;
    foreach (var key in keys)
    {
        var totalFound = new HashSet<int>();

        for (int col = 0; col < 5; col++)
        {
            var height = key.Heights[col];
            var columnFound = new HashSet<int>();

            for (int complement = 5 - height; complement >= 0; complement--)
            {
                if (lockHeights.TryGetValue((col, complement), out var complementLocks))
                    columnFound.UnionWith(complementLocks);
            }

            if (col != 0)
            {
                totalFound.IntersectWith(columnFound);
            }
            else
            {
                totalFound.UnionWith(columnFound);
            }
        }

        matchesFound += totalFound.Count;
    }

    Console.WriteLine($"[Part1:{filepath}] {matchesFound}");
}

Input ParseInput(string filepath)
{
    var content = File.ReadAllText(filepath);
    var schematics = content.Split(["\n\n", "\r\n\r\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    var locks = new List<Schematic>();
    var keys = new List<Schematic>();

    for (int i = 0; i < schematics.Length; i++)
    {
        var schematic = schematics[i];

        var grid = GridUtils.ParseGrid(schematic, c => c.ToString());

        var isKey = GridUtils.EnumerateRows(grid).First().Any(c => c == ".");

        var currentSymbol = isKey ? "." : "#";

        var columns = GridUtils.EnumerateColumns(grid).ToList();
        if (columns.Count != 5 && columns[0].Count() != 7)
            throw new Exception("Invalid schematic size");

        var heights = new int[5];
        for (int j = 0; j < columns.Count; j++)
        {
            var height = columns[j].Skip(1).TakeWhile(c => c == currentSymbol).Count();

            if (isKey)
                height = 5 - height;

            heights[j] = height;
        }

        if (isKey)
            keys.Add(new Schematic(i, heights));
        else
            locks.Add(new Schematic(i, heights));
    }

    return new Input(locks, keys);
}

static Dictionary<(int index, int height), HashSet<int>> GroupHeights(List<Schematic> locks)
{
    return locks
        .SelectMany(l => l.Heights.Select((height, index) => (index, height, l.Id)))
        .GroupBy(
            l => (l.index, l.height),
            l => l.Id)
        .ToDictionary(
            g => g.Key,
            g => g.ToHashSet());
}

record Input(List<Schematic> Locks, List<Schematic> Keys);
record Schematic(int Id, int[] Heights);
