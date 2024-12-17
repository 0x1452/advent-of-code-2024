
using Common;
using Common.AStar;

namespace Day16;

public class ReindeerAStar
{
    private readonly SearchParameters _parameters;
    private readonly Node[,] _nodes;

    public ReindeerAStar(SearchParameters parameters)
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

                if (IsDirectionChange(currentNode.Parent?.Location, currentNode.Location, node.Location))
                    costFromStart += 1000;

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

    private static bool IsDirectionChange(Point? parent, Point current, Point next)
    {
        int dx1;
        int dy1;

        // Starting point doesn't have a parent and is facing east
        if (parent is null)
        {
            dx1 = 1;
            dy1 = 0;
        }
        else
        {
            dx1 = Math.Abs(parent.X - current.X);
            dy1 = Math.Abs(parent.Y - current.Y);
        }


        var dx2 = Math.Abs(current.X - next.X);
        var dy2 = Math.Abs(current.Y - next.Y);

        return dx1 != dx2 || dy1 != dy2;
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

    public List<Node> GetPath()
    {
        var path = new List<Node>();

        if (Search())
        {
            var node = _nodes[_parameters.End.Y, _parameters.End.X];

            while (node.Parent != null)
            {
                path.Add(node);
                node = node.Parent;
            }

            path.Reverse();
        }

        return path;
    }
}
