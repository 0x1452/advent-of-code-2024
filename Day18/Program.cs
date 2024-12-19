using System.Diagnostics;
using Common;
using Common.AStar;

SolvePart1("Input/example.txt", 7, 7, 12);
SolvePart1("Input/input.txt", 71, 71, 1024);

//SolvePart2("Input/example.txt", 7, 7, 12);
//SolvePart2("Input/input.txt", 71, 71, 1024);

SolvePart2BinarySearch("Input/example.txt", 7, 7, 12);
SolvePart2BinarySearch("Input/input.txt", 71, 71, 1024);

void SolvePart1(string filepath, int width, int height, int byteCount)
{
    var grid = ParseInput(filepath, width, height, byteCount);

    Console.WriteLine(GridUtils.GridString(grid, c => c ? "#" : "."));

    var aStar = new AStarOrthogonal(new SearchParameters
    {
        Start = new Point(0, 0),
        End = new Point(grid.GetLength(1) - 1, grid.GetLength(0) - 1),
        Grid = grid
    });

    var path = aStar.GetPath();

    Console.WriteLine($"[Part1:{filepath}:{width}x{height}:{byteCount}] {path.Count}");
}

// very lazy, unoptimized solution but it works :)
void SolvePart2(string filepath, int width, int height, int start)
{
    var grid = ParseInput(filepath, width, height, start);
    var lines = File.ReadAllLines(filepath);

    var sw = Stopwatch.StartNew();
    Point? result = null;

    for (int i = start; i < lines.Length; i++)
    {
        var parts = lines[i].Split(",");
        var x = int.Parse(parts[0]);
        var y = int.Parse(parts[1]);

        grid[y, x] = false;

        var aStar = new AStarOrthogonal(new SearchParameters
        {
            Start = new Point(0, 0),
            End = new Point(grid.GetLength(1) - 1, grid.GetLength(0) - 1),
            Grid = grid
        });

        var path = aStar.GetPath();

        if (path.Count == 0)
        {
            result = new Point(x, y);
            break;
        }
    }
    sw.Stop();

    Console.WriteLine($"[Part2:{filepath}:{width}x{height}:{start}] {result?.X},{result?.Y} {sw.Elapsed.TotalMilliseconds}ms");
}

void SolvePart2BinarySearch(string filepath, int width, int height, int start)
{
    var grid = ParseInput(filepath, width, height, start);
    var lines = File.ReadAllLines(filepath);

    var sw = Stopwatch.StartNew();
    int left = start;
    int right = lines.Length - 1;
    Point? result = null;

    while (left <= right)
    {
        int mid = left + (right - left) / 2;

        bool[,] tempGrid = (bool[,])grid.Clone();
        for (int i = start; i <= mid; i++)
        {
            var parts = lines[i].Split(",");
            var x = int.Parse(parts[0]);
            var y = int.Parse(parts[1]);
            tempGrid[y, x] = false;
        }

        var aStar = new AStarOrthogonal(new SearchParameters
        {
            Start = new Point(0, 0),
            End = new Point(tempGrid.GetLength(1) - 1, tempGrid.GetLength(0) - 1),
            Grid = tempGrid
        });

        var path = aStar.GetPath();

        if (path.Count == 0)
        {
            var parts = lines[mid].Split(",");
            var x = int.Parse(parts[0]);
            var y = int.Parse(parts[1]);
            result = new Point(x, y);
            right = mid - 1;
        }
        else
        {
            left = mid + 1;
        }
    }

    sw.Stop();

    Console.WriteLine($"[Part2BS:{filepath}:{width}x{height}:{start}] {result?.X},{result?.Y} {sw.Elapsed.TotalMilliseconds}ms");
}

bool[,] ParseInput(string filepath, int width, int height, int byteCount)
{
    var grid = new bool[height, width];
    for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            grid[y, x] = true;

    var lines = File.ReadAllLines(filepath);
    foreach (var line in lines.Take(byteCount))
    {
        var parts = line.Split(",");
        var x = int.Parse(parts[0]);
        var y = int.Parse(parts[1]);

        grid[y, x] = false;
    }

    return grid;
}