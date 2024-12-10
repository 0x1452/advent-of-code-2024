
using Common;
using Serilog.Events;
using System.Diagnostics;

var logger = new LoggerSetup().Logger;

Solve("Input/example.txt");
Solve("Input/input.txt");

void Solve(string filepath)
{
    var content = File.ReadAllText(filepath);
    var grid = GridUtils.ParseGrid(content, c => int.Parse(c.ToString()));

    if (logger.IsEnabled(LogEventLevel.Debug))
        logger.Debug("{Grid}", GridUtils.GridString(grid, i => i.ToString()));;

    var sw = Stopwatch.StartNew();
    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);

    var targetValue = 9;

    (int, int)[] directions = [(-1, 0), (1, 0), (0, 1), (0, -1)];

    var sum = 0;
    var distinctSum = 0;
    for (int x = 0; x < cols; x++)
    {
        for (int y = 0; y < rows; y++)
        {
            if (grid[y, x] == 0)
            {
                var targetsReached = new HashSet<(int x, int y)>();
                distinctSum += TraverseTrails(grid, x, y, targetValue, directions, targetsReached);

                sum += targetsReached.Count;

                logger.Debug("[{X}, {Y}] {Score}", x, y, targetsReached.Count);
            }
        }
    }
    sw.Stop();

    logger.Information("[Part1:{Filepath}] {TrailheadScore} {ElapsedMs}ms", filepath, sum, sw.Elapsed.TotalMilliseconds);
    logger.Information("[Part2:{Filepath}] {TrailheadScore} {ElapsedMs}ms", filepath, distinctSum, sw.Elapsed.TotalMilliseconds);
}

int TraverseTrails(int[,] grid, int x, int y, int targetValue, IEnumerable<(int dx, int dy)> directions, HashSet<(int x, int y)> targetsReached)
{
    var startValue = grid[y, x];

    var distinctSum = 0;

    if (startValue == targetValue)
    {
        targetsReached.Add((x, y));
        return 1;
    }

    foreach (var (dx, dy) in directions)
    {
        var nextX = x + dx;
        var nextY = y + dy;

        if (IsOutOfBounds(grid, nextX, nextY))
            continue;

        var nextValue = grid[nextY, nextX];

        if (nextValue == startValue + 1)
            distinctSum += TraverseTrails(grid, nextX, nextY, targetValue, directions, targetsReached);
    }

    return distinctSum;
}

bool IsOutOfBounds(int[,] grid, int x, int y)
{
    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);

    return x < 0
        || x >= cols
        || y < 0
        || y >= rows;
}