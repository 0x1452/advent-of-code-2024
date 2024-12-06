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
        X = playerX,
        Y = playerY,
        Direction = direction
    };
    gridMask[player.Y, player.X] = true;

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

    var (startX, startY) = GetPlayerCoordinates(grid);
    var startDirection = grid[startY, startX] switch
    {
        '^' => Direction.Up,
        'v' => Direction.Down,
        '>' => Direction.Right,
        '<' => Direction.Left,
        _ => throw new NotImplementedException(),
    };

    var player = new Player
    {
        X = startX,
        Y = startY,
        Direction = startDirection
    };
    var traverseMask = new bool[rows, cols];
    TraverseGrid(player, grid, traverseMask);

    var loopCount = 0;
    for (int y = 0; y < rows; y++)
    {
        for (int x = 0; x < cols; x++)
        {
            var isInPlayerPath = traverseMask[y, x];
            if (!isInPlayerPath)
                continue;

            player.X = startX;
            player.Y = startY;
            player.Direction = startDirection;

            var gridMask = new Direction[rows, cols];
            gridMask[player.Y, player.X] = startDirection;

            var originalTile = grid[y, x];

            if (originalTile == '#' || (player.X, player.Y) == (x, y))
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
        var (nextX, nextY) = GetNextStep(player);
        if (IsOutOfBounds(grid, nextX, nextY))
            return false;

        if (IsObstacle(grid, nextX, nextY))
        {
            player.Direction = GetNextDirection(player);
            continue;
        }

        player.X = nextX;
        player.Y = nextY;

        if (gridMask[player.Y, player.X] == player.Direction)
            return true;

        gridMask[player.Y, player.X] = player.Direction;

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
        var (nextX, nextY) = GetNextStep(player);
        if (IsOutOfBounds(grid, nextX, nextY))
        {
            leftGrid = true;
            break;
        }

        if (IsObstacle(grid, nextX, nextY))
        {
            player.Direction = GetNextDirection(player);
            continue;
        }

        player.X = nextX;
        player.Y = nextY;
        gridMask[player.Y, player.X] = true;

        if (logger.IsEnabled(LogEventLevel.Debug))
            logger.Debug("{Grid}", GridUtils.GridString(gridMask, b => b ? "1" : "0") + "\n");
    }
}

(int X, int Y) GetNextStep(Player player)
{
    var nextX = player.Direction switch
    {
        Direction.Right => player.X + 1,
        Direction.Left => player.X - 1,
        _ => player.X
    };

    var nextY = player.Direction switch
    {
        Direction.Up => player.Y - 1,
        Direction.Down => player.Y + 1,
        _ => player.Y
    };

    return (nextX, nextY);
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

bool IsOutOfBounds(char[,] grid, int x, int y)
{
    int rows = grid.GetLength(0);
    int cols = grid.GetLength(1);

    return x >= rows
        || x < 0
        || y >= cols
        || y < 0;
}

bool IsObstacle(char[,] grid, int x, int y)
    => grid[y, x] == '#';

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

class Player
{
    public int X { get; set; }
    public int Y { get; set; }
    public Direction Direction { get; set; }
}
