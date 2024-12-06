
using Common;
using Serilog;

ILogger logger = new LoggerSetup().Logger;


void SolvePart1(string path)
{
    var input = File.ReadAllText(path);
    var grid = GridUtils.ParseGrid(input, c => c);

    IEnumerable<IEnumerable<char>> sequences = new[]
    {
        GridUtils.EnumerateRows(grid),
        GridUtils.EnumerateColumns(grid),
        GridUtils.EnumerateDiagonals(grid),
    }
    .SelectMany(x => x);

    var keyword = "XMAS";

    var sum = 0;
    foreach (var sequence in sequences)
    {
        logger.Verbose(string.Join(" ", sequence));
        sum += CountOccurrences(sequence, keyword);
    }

    logger.Information($"[Part1:{path}] {sum}");
}

void SolvePart2(string path)
{
    var input = File.ReadAllText(path);
    var grid = GridUtils.ParseGrid(input, c => c);

    var rowCount = grid.GetLength(0);
    var columnCount = grid.GetLength(1);

    var keyword = "MAS";

    var sum = 0;
    for (int x = 1; x < rowCount - 1; x++)
    {
        for (int y = 1; y < columnCount - 1; y++)
        {
            if (grid[y, x] == 'A')
            {
                var diagonals = new[]
                {
                    new[] { grid[y - 1, x - 1], grid[y, x], grid[y + 1, x + 1] },
                    new[] { grid[y - 1, x + 1], grid[y, x], grid[y + 1, x - 1] }
                };

                var isXmas = diagonals
                    .All(diagonal => diagonal.SequenceEqual(keyword) || diagonal.SequenceEqual(keyword.Reverse()));

                if (isXmas)
                    sum++;
            }
        }
    }

    logger.Information($"[Part2:{path}] {sum}");
}

static int CountOccurrences(IEnumerable<char> chars, string keyword)
{
    var queue = new Queue<char>(keyword.Length);
    var index = 0;

    var count = 0;
    foreach (var c in chars)
    {
        queue.Enqueue(c);

        if (queue.Count > keyword.Length)
            queue.Dequeue();

        if (queue.Count == keyword.Length && (queue.SequenceEqual(keyword) || queue.SequenceEqual(keyword.Reverse())))
            count++;

        index++;
    }

    return count;
}

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");
SolvePart2("Input/example.txt");
SolvePart2("Input/input.txt");
