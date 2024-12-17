namespace Common.AStar;
public class Node
{
    public required Point Location { get; set; }
    public bool IsWalkable { get; set; }
    public float CostFromStart { get; set; }
    public float EstimatedRemainingCost { get; set; }
    public float TotalEstimatedCost => CostFromStart + EstimatedRemainingCost;
    public NodeState State { get; set; }
    public Node? Parent { get; set; }
}

public enum NodeState
{
    Unknown,
    Open,
    Closed
}
