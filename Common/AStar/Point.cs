namespace Common.AStar;

public class Point(int X, int Y)
{
    public int X { get; set; } = X;
    public int Y { get; set; } = Y;

    public override bool Equals(object? obj)
    {
        return obj is Point point 
            && point.X == X 
            && point.Y == Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}
