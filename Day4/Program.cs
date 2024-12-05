
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
    var keywordReversed = string.Join("", keyword.Reverse());

    var sum = 0;
    foreach (var sequence in sequences)
    {
        logger.Verbose(string.Join(" ", sequence));
        sum += CountOccurrences(sequence, keyword);
        sum += CountOccurrences(sequence, keywordReversed);
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
                var subgrid = GridUtils.GetSubgrid(
                    grid, 
                    startY: y - 1, 
                    startX: x - 1, 
                    width: 3, 
                    height: 3);

                var xOccurrences = CountXOccurences(subgrid, keyword);
                logger.Debug(GridUtils.GridString(subgrid));
                if (xOccurrences == 2)
                    sum++;
            }
        }
    }

    logger.Information($"[Part2:{path}] {sum}");
}

int CountXOccurences(char[,] grid, string keyword)
{
    var keywordReversed = string.Join("", keyword.Reverse());

    var sum = 0;
    foreach (var sequence in GridUtils.EnumerateDiagonals(grid))
    {
        logger.Verbose(string.Join(" ", sequence));
        sum += CountOccurrences(sequence, keyword);
        sum += CountOccurrences(sequence, keywordReversed);
    }

    return sum;
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

        if (queue.Count == keyword.Length && Matches(queue, keyword))
            count++;

        index++;
    }

    return count;
}

static bool Matches(Queue<char> queue, string keyword)
{
    var i = 0;

    foreach (var c in queue)
    {
        if (c != keyword[i])
            return false;

        i++;
    }

    return true;
}


SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");
SolvePart2("Input/example.txt");
SolvePart2("Input/input.txt");
