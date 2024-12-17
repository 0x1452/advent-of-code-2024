using Common;

Dictionary<char, (int dx, int dy)> directions = new()
{
    { '<', (-1,  0) },
    { '>', ( 1,  0) },
    { '^', ( 0, -1) },
    { 'v', ( 0,  1) },
};

SolvePart1("Input/example1.txt");
SolvePart1("Input/example2.txt");
SolvePart1("Input/input.txt");
SolvePart2("Input/example1.txt");
SolvePart2("Input/example2.txt");
SolvePart2("Input/example3.txt");
SolvePart2("Input/input.txt");

void SolvePart1(string filepath)
{
    var (grid, moves, x, y) = ParseInput(filepath);

    foreach (var move in moves)
    {
        //Console.WriteLine(move);
        (x, y) = Move(grid, x, y, move);
    }

    PrintGrid(grid);

    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);

    var sum = 0;
    for (int ix = 0; ix < cols; ix++)
    {
        for (int iy = 0; iy < rows; iy++)
        {
            if (grid[iy, ix] == CellType.Box)
                sum += 100 * iy + ix;
        }
    }

    Console.WriteLine($"[Part1:{filepath}] {sum}");
}

void SolvePart2(string filepath)
{
    var (grid, moves, x, y) = ParseInput(filepath);

    grid = ExtendGrid(grid);
    x *= 2;

    foreach (var move in moves)
    {
        (x, y) = Move(grid, x, y, move);
    }

    PrintGrid(grid);

    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);

    var sum = 0;
    for (int ix = 0; ix < cols; ix++)
    {
        for (int iy = 0; iy < rows; iy++)
        {
            if (grid[iy, ix] == CellType.BoxLeft)
                sum += 100 * iy + ix;
        }
    }

    Console.WriteLine($"[Part2:{filepath}] {sum}");
}

(int x, int y) Move(CellType[,] grid, int x, int y, char direction)
{
    if (!CanMove(grid, x, y, direction))
        return (x, y);

    var currentCell = grid[y, x];

    var (dx, dy) = directions[direction];
    var nextCell = grid[y + dy, x + dx];

    if (nextCell == CellType.Box)
        Move(grid, x + dx, y + dy, direction);

    if (nextCell == CellType.BoxLeft || nextCell == CellType.BoxRight)
    {
        if (direction == '^' || direction == 'v')
        {
            var (otherX, otherY) = GetOtherBoxPart(grid, x + dx, y + dy);
            Move(grid, otherX, otherY, direction);
        }

        Move(grid, x + dx, y + dy, direction);
    }

    nextCell = grid[y + dy, x + dx];

    if (nextCell == CellType.Empty)
    {
        grid[y + dy, x + dx] = currentCell;
        grid[y, x] = CellType.Empty;

        return (x + dx, y + dy);
    }

    return (x, y);
}

(int otherX, int otherY) GetOtherBoxPart(CellType[,] grid, int x, int y)
{
    var (dx, dy) = grid[y, x] == CellType.BoxLeft
        ? directions['>']
        : directions['<'];

    return (x + dx, y + dy);
}

bool CanMove(CellType[,] grid, int x, int y, char direction)
{
    if (GridUtils.IsOutOfBounds(grid, x, y))
        return false;

    var currentCell = grid[y, x];
    if (!IsMovable(currentCell))
        return false;

    var (dx, dy) = directions[direction];
    if (GridUtils.IsOutOfBounds(grid, x + dx, y + dy))
        return false;

    var nextCell = grid[y + dy, x + dx];

    if (nextCell == CellType.Wall)
        return false;

    if (nextCell == CellType.Box)
        return CanMove(grid, x + dx, y + dy, direction);

    if (nextCell == CellType.BoxLeft || nextCell == CellType.BoxRight)
    {
        if (direction == '^' || direction == 'v')
        {
            var (otherX, otherY) = GetOtherBoxPart(grid, x + dx, y + dy);
            if (!CanMove(grid, otherX, otherY, direction))
                return false;
        }

        if (!CanMove(grid, x + dx, y + dy, direction))
            return false;
    }

    return true;
}

static Input ParseInput(string filepath)
{
    var content = File.ReadAllText(filepath).Split(["\n\n", "\r\n\r\n"], StringSplitOptions.RemoveEmptyEntries);
    var lines = content[0].Split(["\n", "\r\n"], StringSplitOptions.RemoveEmptyEntries).Select(line => line.Trim()).ToList();
    int rows = lines.Count;
    int cols = lines[0].Length;
    var grid = new CellType[rows, cols];

    var startX = 0;
    var startY = 0;

    for (int y = 0; y < rows; y++)
    {
        for (int x = 0; x < cols; x++)
        {
            grid[y, x] = lines[y][x] switch
            {
                '#' => CellType.Wall,
                'O' => CellType.Box,
                '@' => CellType.Robot,
                _ => CellType.Empty,
            };

            if (grid[y, x] == CellType.Robot)
            {
                startX = x;
                startY = y;
            }
        }
    }

    var moves = content[1].Replace("\n", "").Replace("\r", "");
    return new Input(grid, moves.ToCharArray(), startX, startY);
}

static CellType[,] ExtendGrid(CellType[,] grid)
{
    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);

    var extendedGrid = new CellType[rows, cols * 2];

    for (int y = 0; y < rows; y++)
    {
        for (int x = 0; x < cols; x++)
        {
            var (first, second) = grid[y, x] switch
            {
                CellType.Wall => (CellType.Wall, CellType.Wall),
                CellType.Box => (CellType.BoxLeft, CellType.BoxRight),
                CellType.Robot => (CellType.Robot, CellType.Empty),
                _ => (CellType.Empty, CellType.Empty)
            };

            extendedGrid[y, x * 2] = first;
            extendedGrid[y, x * 2 + 1] = second;
        }
    }

    return extendedGrid;
}

static bool IsMovable(CellType cellType)
{
    switch (cellType)
    {
        case CellType.Robot:
        case CellType.Box:
        case CellType.BoxLeft:
        case CellType.BoxRight:
            return true;

        default:
            return false;
    }
}

static void PrintGrid(CellType[,] grid)
{
    Console.WriteLine(GridUtils.GridString(grid, c => c switch
    {
        CellType.Wall => "#",
        CellType.Box => "O",
        CellType.BoxLeft => "[",
        CellType.BoxRight => "]",
        CellType.Robot => "@",
        _ => "."
    }));
}

enum CellType
{
    Empty,
    Wall,
    Box,
    BoxLeft,
    BoxRight,
    Robot
}

record Input(CellType[,] grid, char[] moves, int startX, int startY);