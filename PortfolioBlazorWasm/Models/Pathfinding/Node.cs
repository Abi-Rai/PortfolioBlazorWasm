using PortfolioBlazorWasm.Models.Pathfinding.Enums;

namespace PortfolioBlazorWasm.Models.Pathfinding;

public sealed class Node : IEquatable<Node?>, IComparable<Node?>
{
    public Node(int x, int y, int distance, NodeState state = NodeState.None, Node? previousNode = null)
    {
        X = x;
        Y = y;
        Distance = distance;
        Visited = false;
        State = state;
        PreviousNode = previousNode;
        GCost = int.MaxValue;
        HCost = 0;
    }

    public int X { get; }
    public int Y { get; }
    public int Distance { get; set; }
    public bool Visited { get; set; }

    public NodeState State { get; set; }
    public Node? PreviousNode { get; set; }
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost => GCost + HCost;
    public bool Equals(Node? other)
    {
        if (other is null) return false;
        return (X == other.X && Y == other.Y);
    }

    public override bool Equals(object? obj)
    {
        return obj is Node otherPoint && Equals(otherPoint);
    }

    public static bool operator ==(Node left, Node? right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Node left, Node? right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X.GetHashCode(), Y.GetHashCode());
    }

    public int CompareTo(Node? other)
    {
        if (other is null) return 1;
        var compare = Distance.CompareTo(other.Distance);
        if (compare == 0)
        {
            if (this.Equals(other)) return 0;
            var xCompare = X - other.X;
            if (xCompare == 0)
                return Y - other.Y;
            return xCompare;
        }
        return compare;
    }

    public static bool operator <(Node left, Node? right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Node left, Node? right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Node left, Node? right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Node left, Node? right)
    {
        return left.CompareTo(right) >= 0;
    }

}
