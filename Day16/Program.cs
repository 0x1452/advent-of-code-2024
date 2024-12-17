using Common;
using Common.AStar;

var input = File.ReadAllText("Input/example.txt");

var (searchParameters, debugGrid) = ParseInput(input);
var aStar = new AStarOrthogonal(searchParameters);

Console.WriteLine(GridUtils.GridString(debugGrid, c => c.ToString()));
var path = aStar.GetPath();
foreach (var point in path)
{
    debugGrid[point.Y, point.X] = 'x';
}
Console.WriteLine(GridUtils.GridString(debugGrid, c => c.ToString()));


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