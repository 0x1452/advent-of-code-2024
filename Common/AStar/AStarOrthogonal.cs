
namespace Common.AStar;

public class AStarOrthogonal
{
    private readonly SearchParameters _parameters;
    private readonly Node[,] _nodes;

    public AStarOrthogonal(SearchParameters parameters)
    {
        _parameters = parameters;

        var height = _parameters.Grid.GetLength(0);
        var width = _parameters.Grid.GetLength(1);
        _nodes = new Node[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                _nodes[y, x] = new Node
                {
                    Location = new Point(x, y),
                    IsWalkable = _parameters.Grid[y, x],
                };
            }
        }
    }

    private bool Search()
    {
        var openList = new PriorityQueue<Node, float>();
        var startNode = _nodes[_parameters.Start.Y, _parameters.Start.X];
        startNode.CostFromStart = 0;
        startNode.EstimatedRemainingCost = GetTraversalCost(startNode.Location, _parameters.End);
        openList.Enqueue(startNode, startNode.TotalEstimatedCost);

        while (openList.Count > 0)
        {
            var currentNode = openList.Dequeue();
            currentNode.State = NodeState.Closed;

            if (currentNode.Location.X == _parameters.End.X && currentNode.Location.Y == _parameters.End.Y)
                return true;

            var adjacentNodes = GetAdjacentWalkableNodes(currentNode);

            foreach (var node in adjacentNodes)
            {
                if (node.State == NodeState.Closed)
                    continue;

                var traversalCost = GetTraversalCost(node.Location, currentNode.Location);
                var costFromStart = currentNode.CostFromStart + traversalCost;

                if (node.State == NodeState.Open && costFromStart >= node.CostFromStart)
                    continue;

                node.Parent = currentNode;
                node.CostFromStart = costFromStart;
                node.EstimatedRemainingCost = GetTraversalCost(node.Location, _parameters.End);

                if (node.State != NodeState.Open)
                {
                    node.State = NodeState.Open;
                    openList.Enqueue(node, node.TotalEstimatedCost);
                }
            }
        }

        return false;
    }

    private List<Node> GetAdjacentWalkableNodes(Node from)
    {
        var walkableNodes = new List<Node>();
        var nextLocations = GetAdjacentLocations(from.Location);

        foreach (var location in nextLocations)
        {
            if (GridUtils.IsOutOfBounds(_parameters.Grid, location.X, location.Y))
                continue;

            var node = _nodes[location.Y, location.X];
            if (!node.IsWalkable || node.State == NodeState.Closed)
                continue;

            walkableNodes.Add(node);
        }

        return walkableNodes;
    }

    private static float GetTraversalCost(Point from, Point to)
    {
        int deltaX = Math.Abs(from.X - to.X);
        int deltaY = Math.Abs(from.Y - to.Y);

        if ((deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1))
        {
            return 1.0f;
        }

        return (float)Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
    }

    private static IEnumerable<Point> GetAdjacentLocations(Point location)
    {
        foreach (var (dx, dy) in GridUtils.Directions4)
        {
            yield return new Point(location.X + dx, location.Y + dy);
        }
    }

    public List<Point> GetPath()
    {
        var path = new List<Point>();

        if (Search())
        {
            var node = _nodes[_parameters.End.Y, _parameters.End.X];

            while (node.Parent != null)
            {
                path.Add(node.Location);
                node = node.Parent;
            }

            path.Reverse();
        }

        return path;
    }
}
