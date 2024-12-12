using System.ComponentModel;
using System.Text;

namespace Common;

public class GridUtils
{    
    /// <summary> List of (dx, dy) to get the direct neighbors (not including diagonals) </summary>
    public static (int dx, int dy)[] Directions4 = [(-1, 0), (0, 1), (1, 0), (0, -1)];

    /// <summary> List of (dx, dy) to get all neighbors (including diagonals) </summary>
    public static (int dx, int dy)[] Directions8 = [(-1, 0), (-1, 1), (0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1)];

    public static T[,] ParseGrid<T>(string input, Func<char, T> parseElement)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(line => line.Trim()).ToList();
        int rows = lines.Count;
        int cols = lines[0].Length;
        var grid = new T[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                grid[r, c] = parseElement(lines[r][c]);
            }
        }

        return grid;
    }

    public static string GridString<T>(T[,] grid, Func<T, string> toString)
    {
        var sb = new StringBuilder();

        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                sb.Append(toString(grid[r, c]));
            }
            sb.Append("\n");
        }

        return sb.ToString();
    }

    public static IEnumerable<IEnumerable<T>> EnumerateRows<T>(T[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            yield return EnumerateRow(grid, r, cols);
        }
    }

    private static IEnumerable<T> EnumerateRow<T>(T[,] grid, int row, int cols)
    {
        for (int c = 0; c < cols; c++)
        {
            yield return grid[row, c];
        }
    }

    public static IEnumerable<IEnumerable<T>> EnumerateColumns<T>(T[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int c = 0; c < cols; c++)
        {
            yield return EnumerateColumn(grid, c, rows);
        }
    }

    private static IEnumerable<T> EnumerateColumn<T>(T[,] grid, int col, int rows)
    {
        for (int r = 0; r < rows; r++)
        {
            yield return grid[r, col];
        }
    }

    public static IEnumerable<IEnumerable<T>> EnumerateDiagonals<T>(T[,] grid, bool leftToRight = true)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Start from the top-right corner and follow top-to-bottom diagonals
        for (int diagonal = 0; diagonal < rows + cols - 1; diagonal++)
        {
            var startRow = Math.Max(0, diagonal - cols + 1);
            var startColumn = Math.Max(0, cols - 1 - diagonal);
            yield return EnumerateDiagonal(grid, startRow, startColumn, Direction.LeftToRight);
        }

        // Start from the top-left corner and follow top-to-bottom diagonals
        for (int diagonal = 0; diagonal < rows + cols - 1; diagonal++)
        {
            var startRow = Math.Max(0, diagonal - cols + 1);
            var startColumn = Math.Min(diagonal, cols - 1);
            yield return EnumerateDiagonal(grid, startRow, startColumn, Direction.RightToLeft);
        }
    }

    private enum Direction
    {
        LeftToRight,
        RightToLeft,
    }

    private static IEnumerable<T> EnumerateDiagonal<T>(T[,] grid, int startRow, int startColumn, Direction direction)
    {
        var rowCount = grid.GetLength(0);
        var columnCount = grid.GetLength(1);
        var row = startRow;
        var column = startColumn;

        while (row < rowCount && column >= 0 && column < columnCount)
        {
            yield return grid[row, column];
            row++;
            column += direction switch
            {
                Direction.LeftToRight => 1,
                Direction.RightToLeft => -1,
                _ => throw new InvalidEnumArgumentException()
            };
        }
    }

    public static bool IsOutOfBounds<T>(T[,] grid, int x, int y)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        return x < 0
            || x >= cols
            || y < 0
            || y >= rows;
    }

    public static T[,] GetSubgrid<T>(T[,] grid, int startY, int startX, int width, int height)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        if (startY < 0 || startX < 0 || startY + width > cols || startX + height > rows)
            throw new InvalidOperationException("The subgrid dimensions exceed the bounds of the grid");

        var subgrid = new T[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                subgrid[x, y] = grid[startY + x, startX + y];
            }
        }

        return subgrid;
    }

    public static bool TryGet<T>(T[,] grid, int x, int y, out T? value)
    {
        if (IsOutOfBounds(grid, x, y))
        {
            value = default;
            return false;
        }

        value = grid[y, x];
        return true;
    }
}
