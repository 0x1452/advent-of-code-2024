
using Common;
using Serilog;
using System.Diagnostics;

ILogger logger = new LoggerSetup().Logger;

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");
SolvePart1Sobex("Input/example.txt");
SolvePart1Sobex("Input/input.txt");
SolvePart2("Input/example.txt");
SolvePart2("Input/input.txt");


// Not sure how to feel about this one.
// Readable but unnecessarily complicated.
void SolvePart1(string path)
{
    var input = File.ReadAllText(path);
    var grid = GridUtils.ParseGrid(input, c => c);

    var sw = Stopwatch.StartNew();
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
        sum += CountOccurrences(sequence, keyword);
    }

    sw.Stop();
    logger.Information("[Part1:{Path}] {Sum} {ElapsedMs}ms", path, sum, sw.Elapsed.TotalMilliseconds);
}

void SolvePart1Sobex(string path)
{
    var input = File.ReadAllText(path);
    var grid = GridUtils.ParseGrid(input, c => c);

    var sw = Stopwatch.StartNew();
    var rowCount = grid.GetLength(0);
    var columnCount = grid.GetLength(1);

    var keyword = "XMAS";

    // Approach yoinked from SOBEX
    var sum = 0;
    var directions = new List<(int dy, int dx)> { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };
    for (int x = 0; x < rowCount; x++)
    {
        for (int y = 0; y < columnCount; y++)
        {
            if (grid[y, x] == keyword[0])
            {
                sum += CheckDirections(grid, keyword, directions, x, y);
            }
        }
    }

    sw.Stop();
    logger.Information("[Part1 SOBEX:{Path}] {Sum} {ElapsedMs}ms", path, sum, sw.Elapsed.TotalMilliseconds);
}

int CheckDirections(char[,] grid, string keyword, List<(int dy, int dx)> directions, int x, int y)
{
    var sum = 0;
    foreach (var (dy, dx) in directions)
    {
        var isValid = true;

        for (int i = 1; i < keyword.Length; i++)
        {
            var nextX = x + dx * i;
            var nextY = y + dy * i;

            if (IsOutOfBounds(grid, nextX, nextY))
            {
                isValid = false;
                break;
            }

            if (grid[nextY, nextX] != keyword[i])
            {
                isValid = false;
                break;
            }
        }

        if (isValid)
            sum++;
    }

    return sum;
}

bool IsOutOfBounds(char[,] grid, int x, int y)
{
    var rowCount = grid.GetLength(0);
    var columnCount = grid.GetLength(1);

    return x < 0
        || x >= columnCount
        || y < 0
        || y >= rowCount;
}

void SolvePart2(string path)
{
    var input = File.ReadAllText(path);
    var grid = GridUtils.ParseGrid(input, c => c);

    var sw = Stopwatch.StartNew();
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

    sw.Stop(); 
    logger.Information("[Part2:{Path}] {Sum} {ElapsedMs}ms", path, sum, sw.Elapsed.TotalMilliseconds);
}

static int CountOccurrences(IEnumerable<char> chars, string keyword)
{
    var queue = new Queue<char>(keyword.Length);

    var count = 0;
    foreach (var c in chars)
    {
        queue.Enqueue(c);


        if (queue.Count > keyword.Length)
            queue.Dequeue();

        if (queue.Count == keyword.Length)
        {
            var matchesKeyword = true;
            var matchesReversed = true;

            var index = 0;

            foreach (var item in queue)
            {
                if (matchesKeyword && item != keyword[index])
                    matchesKeyword = false;
                if (matchesReversed && item != keyword[keyword.Length - 1 - index])
                    matchesReversed = false;

                index++;

                if (!matchesKeyword && !matchesReversed)
                    break;
            }

            if (matchesKeyword || matchesReversed)
                count++;
        }
    }

    return count;
}

