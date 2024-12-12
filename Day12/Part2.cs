using Common;

namespace Day12;
public class Part2
{
    public static void Solve(string filepath)
    {
        var grid = ParseInput(filepath);

        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        var visitedMask = new bool[rows, cols];

        int totalPrice = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (visitedMask[y, x])
                    continue;

                totalPrice += GetRegionPrice(grid, new Coordinates(x, y), visitedMask);
            }
        }

        Console.WriteLine($"[Part2:{filepath}] {totalPrice}");
    }

    enum CellType
    {
        Empty,
        Perimeter,
        Area
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    static int GetRegionPrice(char[,] grid, Coordinates startPoint, bool[,] overallVisitedMask)
    {
        var area = 0;
        var perimeter = 0;

        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        var localVisitedMask = new bool[rows, cols];
        var perimeterMask = new CellType[rows + 2, cols + 2];

        var targetValue = grid[startPoint.Y, startPoint.X];
        var queue = new Queue<Coordinates>();
        queue.Enqueue(startPoint);
        perimeterMask[startPoint.Y + 1, startPoint.X + 1] = CellType.Area;
        overallVisitedMask[startPoint.Y, startPoint.X] = true;
        localVisitedMask[startPoint.Y, startPoint.X] = true;

        while (queue.Count > 0)
        {
            var point = queue.Dequeue();

            area++;
            foreach (var (x, y) in EnumerateNeighbors(grid, point.X, point.Y))
            {
                if (GridUtils.IsOutOfBounds(grid, x, y) || grid[y, x] != targetValue)
                {
                    perimeter++;
                    perimeterMask[y + 1, x + 1] = CellType.Perimeter;
                    continue;
                }

                if (localVisitedMask[y, x])
                    continue;

                perimeterMask[y + 1, x + 1] = CellType.Area;
                localVisitedMask[y, x] = true;
                overallVisitedMask[y, x] = true;

                queue.Enqueue(new Coordinates(x, y));
            }
        }

        var sides = CountRows(perimeterMask) + CountColumns(perimeterMask);

        //Console.WriteLine($"{targetValue}: Area {area}, Sides {sides}, Perimeter {perimeter}");
        return area * sides;
    }

    private static int CountRows(CellType[,] perimeterMask)
    {
        var sides = 0;
        var rows = perimeterMask.GetLength(0);
        var cols = perimeterMask.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            var isSide = false;

            var directionsWithArea = new HashSet<Direction>();

            for (int x = 0; x < cols; x++)
            {
                var cell = perimeterMask[y, x];
                var isPerimeter = cell == CellType.Perimeter;

                if (!isSide && !isPerimeter)
                    continue;

                if (isSide && !isPerimeter)
                {
                    sides += directionsWithArea.Count;
                    isSide = false;
                    directionsWithArea.Clear();
                    continue;
                }

                GridUtils.TryGet(perimeterMask, x, y - 1, out var cellUp);
                GridUtils.TryGet(perimeterMask, x, y + 1, out var cellDown);
                if (cellUp != CellType.Area && cellDown != CellType.Area)
                    continue;

                isSide = true;

                if (cellUp == CellType.Area) directionsWithArea.Add(Direction.Up);
                if (cellDown == CellType.Area) directionsWithArea.Add(Direction.Down);
            }
        }

        return sides;
    }

    private static int CountColumns(CellType[,] perimeterMask)
    {
        var sides = 0;
        var rows = perimeterMask.GetLength(0);
        var cols = perimeterMask.GetLength(1);

        for (int x = 0; x < cols; x++)
        {
            var isSide = false;

            var directionsWithArea = new HashSet<Direction>();

            for (int y = 0; y < rows; y++)
            {
                var cell = perimeterMask[y, x];
                var isPerimeter = cell == CellType.Perimeter;

                if (!isSide && !isPerimeter)
                    continue;

                if (isSide && !isPerimeter)
                {
                    sides += directionsWithArea.Count;
                    isSide = false;
                    directionsWithArea.Clear();
                    continue;
                }

                GridUtils.TryGet(perimeterMask, x - 1, y, out var cellLeft);
                GridUtils.TryGet(perimeterMask, x + 1, y, out var cellRight);
                if (cellLeft != CellType.Area && cellRight != CellType.Area)
                    continue;

                isSide = true;

                if (cellLeft == CellType.Area) directionsWithArea.Add(Direction.Left);
                if (cellRight == CellType.Area) directionsWithArea.Add(Direction.Right);
            }
        }

        return sides;
    }

    static char[,] ParseInput(string filepath)
    {
        var content = File.ReadAllText(filepath);
        return GridUtils.ParseGrid(content, c => c);
    }

    static IEnumerable<(int x, int y)> EnumerateNeighbors(char[,] grid, int x, int y)
    {
        foreach (var (dx, dy) in GridUtils.Directions4)
        {
            yield return (x + dx, y + dy);
        }
    }

    record Coordinates(int X, int Y);
}
