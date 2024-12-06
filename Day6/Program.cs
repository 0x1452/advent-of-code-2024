using Common;
using Serilog;
using Serilog.Events;

ILogger logger = new LoggerSetup().Logger;

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");
SolvePart2("Input/example.txt");
SolvePart2("Input/input.txt");

void SolvePart1(string path)
{
    var content = File.ReadAllText(path);

    var grid = GridUtils.ParseGrid(content, c => c);

    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);

    var gridMask = new bool[rows, cols];

    var (playerX, playerY) = GetPlayerCoordinates(grid);
    var direction = grid[playerY, playerX] switch
    {
        '^' => Direction.Up,
        'v' => Direction.Down,
        '>' => Direction.Right,
        '<' => Direction.Left,
        _ => throw new NotImplementedException(),
    };

    var player = new Player
    {
        Coordinates = new Coordinates(playerX, playerY),
        Direction = direction
    };
    gridMask[player.Coordinates.Y, player.Coordinates.X] = true;

    TraverseGrid(player, grid, gridMask);

    var visitedCount = gridMask.Cast<bool>().Count(tileVisited => tileVisited);
    logger.Information("[Part1:{Path}] {VisitedCount}", path, visitedCount);
}

void SolvePart2(string path)
{
    var content = File.ReadAllText(path);

    var grid = GridUtils.ParseGrid(content, c => c);

    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);

    var (playerX, playerY) = GetPlayerCoordinates(grid);
    var direction = grid[playerY, playerX] switch
    {
        '^' => Direction.Up,
        'v' => Direction.Down,
        '>' => Direction.Right,
        '<' => Direction.Left,
        _ => throw new NotImplementedException(),
    };


    var loopCount = 0;
    for (int y = 0; y < rows; y++)
    {
        for (int x = 0; x < cols; x++)
        {
            var player = new Player
            {
                Coordinates = new Coordinates(playerX, playerY),
                Direction = direction
            };
            var gridMask = new Direction[rows, cols];
            gridMask[player.Coordinates.Y, player.Coordinates.X] = direction;

            var originalTile = grid[y, x];

            if (originalTile == '#' || player.Coordinates == new Coordinates(x, y))
                continue;

            grid[y, x] = '#';

            if (IsEndlessLoop(player, grid, gridMask))
                loopCount++;

            grid[y, x] = originalTile;
        }
    }

    logger.Information("[Part2:{Path}] {LoopCount}", path, loopCount);
}

bool IsEndlessLoop(Player player, char[,] grid, Direction[,] gridMask)
{
    while (true)
    {
        var nextStep = GetNextStep(player);
        if (IsOutOfBounds(grid, nextStep))
            return false;

        if (IsObstacle(grid, nextStep))
        {
            player.Direction = GetNextDirection(player);
            continue;
        }

        player.Coordinates = nextStep;

        if (gridMask[player.Coordinates.Y, player.Coordinates.X] == player.Direction)
            return true;

        gridMask[player.Coordinates.Y, player.Coordinates.X] = player.Direction;

        if (logger.IsEnabled(LogEventLevel.Debug))
        {
            logger.Debug("{Grid}", GridUtils.GridString(gridMask, d => d switch
            {
                Direction.None => ".",
                Direction.Up => "^",
                Direction.Down => "v",
                Direction.Left => "<",
                Direction.Right => ">",
                _ => "?"
            }));
        }
    }
}

void TraverseGrid(Player player, char[,] grid, bool[,] gridMask)
{
    var leftGrid = false;
    while (!leftGrid)
    {
        var nextStep = GetNextStep(player);
        if (IsOutOfBounds(grid, nextStep))
        {
            leftGrid = true;
            break;
        }

        if (IsObstacle(grid, nextStep))
        {
            player.Direction = GetNextDirection(player);
            continue;
        }

        player.Coordinates = nextStep;
        gridMask[player.Coordinates.Y, player.Coordinates.X] = true;

        if (logger.IsEnabled(LogEventLevel.Debug))
            logger.Debug("{Grid}", GridUtils.GridString(gridMask, b => b ? "1" : "0") + "\n");
    }
}

Coordinates GetNextStep(Player player)
{
    var nextX = player.Direction switch
    {
        Direction.Up or Direction.Down => player.Coordinates.X,
        Direction.Right => player.Coordinates.X + 1,
        Direction.Left => player.Coordinates.X - 1,
        _ => throw new NotImplementedException(),
    };

    var nextY = player.Direction switch
    {
        Direction.Left or Direction.Right => player.Coordinates.Y,
        Direction.Up => player.Coordinates.Y - 1,
        Direction.Down => player.Coordinates.Y + 1,
        _ => throw new NotImplementedException(),
    };

    return new Coordinates(nextX, nextY);
}

Direction GetNextDirection(Player player)
    => player.Direction switch
    {
        Direction.Up => Direction.Right,
        Direction.Down => Direction.Left,
        Direction.Left => Direction.Up,
        Direction.Right => Direction.Down,
        _ => throw new NotImplementedException()
    };

bool IsOutOfBounds(char[,] grid, Coordinates coordinates)
{
    int rows = grid.GetLength(0);
    int cols = grid.GetLength(1);

    return coordinates.X >= rows
        || coordinates.X < 0
        || coordinates.Y >= cols
        || coordinates.Y < 0;
}

bool IsObstacle(char[,] grid, Coordinates coordinates)
    => grid[coordinates.Y, coordinates.X] == '#';

(int x, int y) GetPlayerCoordinates(char[,] grid)
{    
    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);

    int playerX, playerY;
    for (int x = 0; x < cols; x++)
    {
        for (int y = 0; y < rows; y++)
        {
            var gridValue = grid[y, x];

            if (gridValue == '^' || gridValue == '>' || gridValue == 'v' || gridValue == '<')
            {
                playerX = x;
                playerY = y;
                return (playerX, playerY);
            }
        }
    }

    return (-1, -1);
}

enum Direction
{
    None,
    Up,
    Down,
    Left,
    Right
}

record Coordinates(int X, int Y);
class Player
{
    public Coordinates Coordinates { get; set; } = new(0, 0);
    public Direction Direction { get; set; }
}
