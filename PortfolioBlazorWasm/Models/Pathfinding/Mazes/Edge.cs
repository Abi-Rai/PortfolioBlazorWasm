namespace PortfolioBlazorWasm.Models.Pathfinding.Mazes;

public class Edge
{
    public Edge(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }
}
