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

void SolvePart1(string filepath)
{
    var (grid, moves, x, y) = ParseInput(filepath);

    foreach (var move in moves)
    {
        //Console.WriteLine(move);
        (x, y) = Move(grid, x, y, move);
    }

    Console.WriteLine(GridUtils.GridString(grid, c => c switch
    {
        CellType.Wall => "#",
        CellType.Box => "O",
        CellType.Robot => "@",
        _ => "."
    }));

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

(int x, int y) Move(CellType[,] grid, int x, int y, char direction)
{
    var currentCell = grid[y, x];
    if (currentCell != CellType.Robot && currentCell != CellType.Box)
        return (x, y);

    var (dx, dy) = directions[direction];
    var nextIsOutOfBounds = GridUtils.IsOutOfBounds(grid, x + dx, y + dy);

    if (nextIsOutOfBounds)
        return (x, y);

    var nextCell = grid[y + dy, x + dx];

    if (nextCell == CellType.Wall)
        return (x, y);

    if (nextCell == CellType.Box)
        Move(grid, x + dx, y + dy, direction);

    nextCell = grid[y + dy, x + dx];

    if (nextCell == CellType.Empty)
    {
        grid[y + dy, x + dx] = currentCell;
        grid[y, x] = CellType.Empty;

        return (x + dx, y + dy);
    }

    return (x, y);
}

static Input ParseInput(string filepath)
{
    var content = File.ReadAllText(filepath).Split("\n\n");
    var lines = content[0].Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(line => line.Trim()).ToList();
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

    var moves = content[1].Replace("\n", "");
    return new Input(grid, moves.ToCharArray(), startX, startY);
}

enum CellType
{
    Empty,
    Wall,
    Box,
    Robot
}

record Input(CellType[,] grid, char[] moves, int startX, int startY);