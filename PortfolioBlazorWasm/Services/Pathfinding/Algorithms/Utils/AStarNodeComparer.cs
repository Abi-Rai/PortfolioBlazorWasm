using PortfolioBlazorWasm.Models.Pathfinding;

namespace PortfolioBlazorWasm.Services.Pathfinding.Algorithms.Utils;

public class AStarNodeComparer : IComparer<Node>
{
    public int Compare(Node? x, Node? y)
    {
        if (x is null || y is null)
        {
            return 0;
        }
        int compare = x.FCost.CompareTo(y.FCost);
        if (compare == 0)
        {
            compare = x.HCost.CompareTo(y.HCost);
        }
        return compare;
    }
}