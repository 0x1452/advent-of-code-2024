using Common;
using Common.AStar;
using Day16;


SolvePart1("Input/example.txt");
SolvePart1("Input/example2.txt");
SolvePart1("Input/input.txt");

void SolvePart1(string filepath)
{
    var input = File.ReadAllText(filepath);
    var (searchParameters, debugGrid) = ParseInput(input);
    var aStar = new ReindeerAStar(searchParameters);

    var path = aStar.GetPath();
    foreach (var node in path)
    {
        debugGrid[node.Location.Y, node.Location.X] = 'x';
    }
    Console.WriteLine(GridUtils.GridString(debugGrid, c => c.ToString()));

    Console.WriteLine($"[Part1:{filepath}] {path.Last().CostFromStart}");
}

static (SearchParameters, char[,]) ParseInput(string input)
{
    var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(line => line.Trim()).ToList();
    int rows = lines.Count;
    int cols = lines[0].Length;
    var grid = new bool[rows, cols];
    var debugGrid = new char[rows, cols];

    Point? start = null;
    Point? end = null;

    for (int y = 0; y < rows; y++)
    {
        for (int x = 0; x < cols; x++)
        {
            var currentChar = lines[y][x];
            grid[y, x] = currentChar != '#';
            debugGrid[y, x] = currentChar;

            if (currentChar == 'S')
                start = new Point(x, y);
            else if (currentChar == 'E')
                end = new Point(x, y);
        }
    }

    var searchParameters = new SearchParameters
    {
        Grid = grid,
        Start = start,
        End = end
    };

    return (searchParameters, debugGrid);
}