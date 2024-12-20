using Common;
using Common.AStar;

SolvePart1("Input/example.txt", 64);
SolvePart1("Input/example.txt", 40);
SolvePart1("Input/example.txt", 38);
SolvePart1("Input/example.txt", 36);
SolvePart1("Input/example.txt", 20);
SolvePart1("Input/example.txt", 12);
SolvePart1("Input/example.txt", 10);
SolvePart1("Input/example.txt", 8);
SolvePart1("Input/example.txt", 6);
SolvePart1("Input/example.txt", 4);
SolvePart1("Input/example.txt", 2);
SolvePart1("Input/input.txt", 100);

void SolvePart1(string filepath, int shortcutThreshold)
{
    var (grid, start, end) = ParseInput(filepath);

    var aStar = new AStarOrthogonal(new SearchParameters
    {
        Grid = grid,
        Start = start,
        End = end
    });

    var regularPath = aStar.GetPath();

    //PrintPath(grid, regularPath);

    var countShortcuts = CountShortcuts(grid, regularPath, shortcutThreshold);
    Console.WriteLine($"[Part1:{filepath}:{shortcutThreshold}] {countShortcuts}");
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