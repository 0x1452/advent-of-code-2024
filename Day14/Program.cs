using Common;
using System.Text.RegularExpressions;

var inputRegex = new Regex(@"p=(-?\d+),(-?\d+) v=(-?\d+),(-?\d+)");

SolvePart1("Input/example.txt", gridLength: 11, gridHeight: 7, iterations: 100);
SolvePart1("Input/input.txt", gridLength: 101, gridHeight: 103, iterations: 100);
//SolvePart1("Input/debug.txt", gridLength: 11, gridHeight: 7, iterations: 5);
//ExplorePart2("Input/input.txt", gridLength: 101, gridHeight: 103, start: 600, iterations: 10000);

ExplorePart2("Input/input.txt", gridLength: 101, gridHeight: 103, start: 325, iterations: 100000, steps: 101);
//ExplorePart2("Input/input.txt", gridLength: 101, gridHeight: 103, start: 10021, iterations: 100000, steps: 101);
// 325
// 426
// 6587

// "manually" solved part 2:
// - iterate over grid states and print to (zoomed out) console
// - notice repeating pattern every `101` iterations
// - iterate over grid state in steps of `101` and check console until you see the tree
// - win


void ExplorePart2(string filepath, int gridLength, int gridHeight, int start, int iterations, int steps)
{
    var robots = ParseInput(filepath);

    for (int i = start; i < start + iterations; i += steps)
    {
        var debugGrid = new int[gridHeight, gridLength];

        foreach (var robot in robots)
        {
            var finalX = MathMod(robot.X + robot.VX * i, gridLength);
            var finalY = MathMod(robot.Y + robot.VY * i, gridHeight);

            debugGrid[finalY, finalX] += 1;
        }

        Console.WriteLine(i);
        Console.WriteLine(GridUtils.GridString(debugGrid, c => $"{(c == 0 ? "." : c)} "));

        Task.Delay(100).Wait();
    }
}

void SolvePart1(string filepath, int gridLength, int gridHeight, int iterations)
{
    var robots = ParseInput(filepath);

    var robotPositions = new Dictionary<(int x, int y), int>();
    var debugGrid = new int[gridHeight, gridLength];

    foreach (var robot in robots)
    {
        var finalX = MathMod(robot.X + robot.VX * iterations, gridLength);
        var finalY = MathMod(robot.Y + robot.VY * iterations, gridHeight);

        robotPositions[(finalX, finalY)] = robotPositions.GetValueOrDefault((finalX, finalY), 0) + 1;
        debugGrid[finalY, finalX] += 1;
    }

    //Console.WriteLine(GridUtils.GridString(debugGrid, c => $"{c} "));

    var quadrantCount = new Dictionary<Quadrant, int>();

    foreach (var ((x, y), count) in robotPositions)
    {
        var quadrant = GetQuadrant(gridLength, gridHeight, x, y);

        quadrantCount[quadrant] = quadrantCount.GetValueOrDefault(quadrant, 0) + count;
    }

    var result = 1;
    foreach (var (quadrant, count) in quadrantCount)
    {
        //Console.WriteLine($"{quadrant}: {count}");
        if (quadrant != Quadrant.None)
            result *= count;
    }
    Console.WriteLine($"[Part1:{gridLength}x{gridHeight}:{iterations}:{filepath}] {result}");
}

static int MathMod(int a, int b)
{
    return (Math.Abs(a * b) + a) % b;
}

Quadrant GetQuadrant(int gridLength, int gridHeight, int x, int y)
{
    var middleColumn = gridLength / 2;
    var middleRow = gridHeight / 2;

    var isOnMiddleColumn = gridLength % 2 == 1 && x == middleColumn;
    var isOnMiddleRow = gridHeight % 2 == 1 && y == middleRow;
    if (isOnMiddleColumn || isOnMiddleRow)
        return Quadrant.None;

    var leftOfCenter = x < middleColumn;
    var aboveCenter = y < middleRow;

    return (leftOfCenter, aboveCenter) switch
    {
        (true, true) => Quadrant.UpLeft,
        (false, true) => Quadrant.UpRight,
        (true, false) => Quadrant.DownLeft,
        (false, false) => Quadrant.DownRight
    };
}

List<Robot> ParseInput(string filepath)
{
    var robots = new List<Robot>();
    var lines = File.ReadAllLines(filepath);

    foreach (var line in lines)
    {
        var match = inputRegex.Match(line);
        var x = int.Parse(match.Groups[1].Value);
        var y = int.Parse(match.Groups[2].Value);
        var vx = int.Parse(match.Groups[3].Value);
        var vy = int.Parse(match.Groups[4].Value);

        robots.Add(new Robot(x, y, vx, vy));
    }

    return robots;
}

enum Quadrant
{
    None,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight
}

class Robot(int X, int Y, int VX, int VY)
{
    public int X { get; set; } = X;
    public int Y { get; set; } = Y;
    public int VX { get; set; } = VX;
    public int VY { get; set; } = VY;
}
