using System.Diagnostics;
using Common;
using Common.AStar;

int[] part1ExampleThresholds = [64, 40, 38, 36, 20, 12, 10, 8, 6, 4, 2];
foreach (var threshold in part1ExampleThresholds)
{
    SolvePart1("Input/example.txt", threshold);
}
SolvePart1("Input/input.txt", 100);


int[] part2ExampleThresholds = [76, 74, 72, 70, 68, 66, 64];
foreach (var threshold in part2ExampleThresholds)
{
    SolvePart2("Input/example.txt", 20, threshold);
}
SolvePart2("Input/input.txt", 20, 100);

void SolvePart1(string filepath, int shortcutThreshold)
{
    var (grid, start, end) = ParseInput(filepath);

    var sw = Stopwatch.StartNew();

    var aStar = new AStarOrthogonal(new SearchParameters
    {
        Grid = grid,
        Start = start,
        End = end
    });

    var regularPath = aStar.GetPath();

    //PrintPath(grid, regularPath);

    var countShortcuts = CountShortcuts(grid, regularPath, shortcutThreshold);

    sw.Stop();

    Console.WriteLine($"[Part1:{filepath}:{shortcutThreshold}] {countShortcuts} {sw.Elapsed.TotalMilliseconds}ms");
}

void SolvePart2(string filepath, int radius, int shortcutThreshold)
{
    var (grid, start, end) = ParseInput(filepath);

    var sw = Stopwatch.StartNew();

    var aStar = new AStarOrthogonal(new SearchParameters
    {
        Grid = grid,
        Start = start,
        End = end
    });

    var regularPath = aStar.GetPath();
    regularPath.Insert(0, start);

    //PrintPath(grid, regularPath);

    //Console.WriteLine(regularPath.Count);

    var countShortcuts = CountShortcutsInRadius(grid, regularPath.ToHashSet(), radius, shortcutThreshold);

    sw.Stop();

    Console.WriteLine($"[Part2:{filepath}:{shortcutThreshold}] {countShortcuts} {sw.Elapsed.TotalMilliseconds}ms");
}

int CountShortcuts(bool[,] isWalkableGrid, List<Point> path, int shortcutThreshold)
{
    var shortcutCount = 0;

    var distanceGrid = new int[isWalkableGrid.GetLength(0), isWalkableGrid.GetLength(1)];
    for (int i = 0; i < path.Count; i++)
    {
        var point = path[i];
        distanceGrid[point.Y, point.X] = i;
    }

    var visitedWalls = new HashSet<(int, int)>();

    foreach (var tile in path)
    {
        // Get neighboring walls
        foreach (var (dx, dy) in GridUtils.Directions4)
        {
            var (nx, ny) = (tile.X + dx, tile.Y + dy);

            if (GridUtils.IsOutOfBounds(isWalkableGrid, nx, ny))
                continue;

            if (isWalkableGrid[ny, nx])
                continue;

            var alreadyVisited = !visitedWalls.Add((nx, ny));
            if (alreadyVisited)
                continue;

            // Gather distance covered by neighboring walkable cells
            var walkableNeighbors = new List<int>();
            foreach (var (dnx, dny) in GridUtils.Directions4)
            {
                if (GridUtils.TryGet(isWalkableGrid, nx + dnx, ny + dny, out var isWalkable) && isWalkable)
                    walkableNeighbors.Add(distanceGrid[ny + dny, nx + dnx]);
            }

            if (walkableNeighbors.Count <= 1)
                continue;

            if (walkableNeighbors.Max() - walkableNeighbors.Min() > shortcutThreshold)
                shortcutCount++;
        }
    }

    return shortcutCount;
}

int CountShortcutsInRadius(bool[,] isWalkableGrid, HashSet<Point> path, int radius, int shortcutThreshold)
{
    var shortcutCount = 0;

    var distanceGrid = new int[isWalkableGrid.GetLength(0), isWalkableGrid.GetLength(1)];
    var i = 0;
    foreach (var point in path)
    {
        distanceGrid[point.Y, point.X] = i;
        i++;
    }

    var offsets = PrecomputeOffsets(radius);

    foreach (var point in path)
    {
        var startDistance = distanceGrid[point.Y, point.X];

        var reachablePoints = GetReachablePoints(point.X, point.Y, offsets)
            .Where(p => path.Contains(p.Point));

        var pointsAboveThreshold = reachablePoints
            .Where(p => distanceGrid[p.Point.Y, p.Point.X] - startDistance - p.StepsAway >= shortcutThreshold);

        if (pointsAboveThreshold.Any())
        {
            //Console.WriteLine($"({point.X}, {point.Y}): " + string.Join(", ", pointsAboveThreshold.Select(p => $"({p.Point.X}, {p.Point.Y}) saves {distanceGrid[p.Point.Y, p.Point.X] - startDistance - p.StepsAway}")));
            shortcutCount += pointsAboveThreshold.Count();

            //DrawGrid(isWalkableGrid, distanceGrid, pointsAboveThreshold.Select(p => p.Point).Concat([point]), ConsoleColor.Red);
        }
    }

    return shortcutCount;
}

static void DrawGrid(bool[,] grid, int[,] distanceGrid, IEnumerable<Point> highlightedPoints, ConsoleColor highlightColor)
{
    var highlightedSet = new HashSet<Point>(highlightedPoints);

    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);

    for (int y = 0; y < rows; y++)
    {
        for (int x = 0; x < cols; x++)
        {
            if (highlightedSet.Contains(new Point(x, y)))
            {
                Console.ForegroundColor = highlightColor;
                Console.Write(grid[y, x]
                    ? $"{distanceGrid[y, x]:D2} "
                    : "## ");
            }
            else
            {
                Console.ResetColor();
                Console.Write(grid[y, x]
                    ? $"{distanceGrid[y, x]:D2} "
                    : "## ");
            }
        }
        Console.WriteLine();
    }

    Console.ResetColor();
    Console.WriteLine();
}


static List<(int dx, int dy)> PrecomputeOffsets(int steps)
{
    var offsets = new List<(int, int)>();
    for (int dx = -steps; dx <= steps; dx++)
    {
        int maxDy = steps - Math.Abs(dx);
        for (int dy = -maxDy; dy <= maxDy; dy++)
        {
            offsets.Add((dx, dy));
        }
    }
    return offsets;
}

static IEnumerable<(Point Point, int StepsAway)> GetReachablePoints(int startX, int startY, List<(int, int)> offsets)
{
    foreach (var (dx, dy) in offsets)
    {
        yield return (new Point(startX + dx, startY + dy), Math.Abs(dx) + Math.Abs(dy));
    }
}

void PrintPath(bool[,] grid, List<Point> path)
{
    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);

    var debugGrid = new char[rows, cols];

    for (int y = 0; y < rows; y++)
    {
        for (int x = 0; x < cols; x++)
        {
            debugGrid[y, x] = grid[y, x] ? '.' : '#';
        }
    }

    foreach (var point in path)
    {
        debugGrid[point.Y, point.X] = 'x';
    }

    Console.WriteLine(GridUtils.GridString(debugGrid, c => c.ToString()));
}

static Input ParseInput(string filepath)
{
    var content = File.ReadAllText(filepath);

    Point? start = null;
    Point? end = null;

    var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(line => line.Trim()).ToList();
    int rows = lines.Count;
    int cols = lines[0].Length;
    var grid = new bool[rows, cols];

    for (int y = 0; y < rows; y++)
    {
        for (int x = 0; x < cols; x++)
        {
            grid[y, x] = lines[y][x] != '#';

            if (lines[y][x] == 'S')
                start = new Point(x, y);

            if (lines[y][x] == 'E')
                end = new Point(x, y);
        }
    }

    return new Input(grid, start!, end!);
}
record Input(bool[,] Grid, Point Start, Point End);