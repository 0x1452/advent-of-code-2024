
using Common;
using System.Diagnostics;

var logger = new LoggerSetup().Logger;

(int, int)[] directions = [(-1, 0), (1, 0), (0, 1), (0, -1)];

Solve("Input/example.txt");
Solve("Input/input.txt");

void Solve(string filepath)
{
    var content = File.ReadAllText(filepath);
    var grid = GridUtils.ParseGrid(content, c => int.Parse(c.ToString()));

    logger.Debug("{Grid}", GridUtils.GridString(grid, i => i.ToString()));;

    var sw = Stopwatch.StartNew();
    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);

    var targetCount = 0;
    var distinctCount = 0;
    for (int x = 0; x < cols; x++)
    {
        for (int y = 0; y < rows; y++)
        {
            if (grid[y, x] == 0)
            {
                var targetsReached = new HashSet<(int x, int y)>();
                distinctCount += TraverseTrails(grid, x, y, targetsReached);

                targetCount += targetsReached.Count;

                logger.Debug("[{X}, {Y}] {Score}", x, y, targetsReached.Count);
            }
        }
    }
    sw.Stop();

    logger.Information("[Part1:{Filepath}] {TrailheadScore} {ElapsedMs}ms", filepath, targetCount, sw.Elapsed.TotalMilliseconds);
    logger.Information("[Part2:{Filepath}] {TrailheadScore} {ElapsedMs}ms", filepath, distinctCount, sw.Elapsed.TotalMilliseconds);
}

int TraverseTrails(int[,] grid, int x, int y, HashSet<(int x, int y)> targetsReached)
{
    var startValue = grid[y, x];

    var distinctSum = 0;

    if (startValue == 9)
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
            distinctSum += TraverseTrails(grid, nextX, nextY, targetsReached);
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