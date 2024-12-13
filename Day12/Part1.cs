using Common;

namespace Day12;
public class Part1
{
    public static void Solve(string filepath)
    {
        var grid = ParseInput(filepath);

        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        var visitedMask = new bool[rows, cols];


        var totalPrice = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (visitedMask[y, x])
                    continue;

                totalPrice += GetRegionPrice(grid, new Coordinates(x, y), visitedMask);
            }
        }

        Console.WriteLine($"[Part1:{filepath}] {totalPrice}");
    }

    static int GetRegionPrice(char[,] grid, Coordinates startPoint, bool[,] overallVisitedMask)
    {
        var area = 0;
        var perimeter = 0;

        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        var localVisitedMask = new bool[rows, cols];

        var targetValue = grid[startPoint.Y, startPoint.X];
        var queue = new Queue<Coordinates>();
        queue.Enqueue(startPoint);
        overallVisitedMask[startPoint.Y, startPoint.X] = true;
        localVisitedMask[startPoint.Y, startPoint.X] = true;

        while (queue.Count > 0)
        {
            var point = queue.Dequeue();

            area++;
            foreach (var (x, y) in EnumerateNeighbors(point.X, point.Y))
            {
                if (GridUtils.IsOutOfBounds(grid, x, y))
                {
                    perimeter++;
                    continue;
                }

                if (grid[y, x] != targetValue)
                {
                    perimeter++;
                    continue;
                }

                if (localVisitedMask[y, x])
                    continue;

                localVisitedMask[y, x] = true;
                overallVisitedMask[y, x] = true;

                queue.Enqueue(new Coordinates(x, y));
            }
        }

        //Console.WriteLine($"{targetValue}: Area {area}, Perimeter {perimeter}");
        return area * perimeter;
    }

    static char[,] ParseInput(string filepath)
    {
        var content = File.ReadAllText(filepath);
        return GridUtils.ParseGrid(content, c => c);
    }

    static IEnumerable<(int x, int y)> EnumerateNeighbors(int x, int y)
    {
        foreach (var (dx, dy) in GridUtils.Directions4)
        {
            yield return (x + dx, y + dy);
        }
    }

    record Coordinates(int X, int Y);
}
