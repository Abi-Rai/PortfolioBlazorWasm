using PortfolioBlazorWasm.Models.Pathfinding;
using PortfolioBlazorWasm.Models.Pathfinding.Enums;
using PortfolioBlazorWasm.Services.PathfindingService.Algorithms.Utils;

namespace PortfolioBlazorWasm.Services.PathfindingService.Algorithms;

public class AStar : BaseAlgorithm
{
    public AStar(IPathfindingRunner pathRunner) : base(pathRunner)
    {
    }
    public override async Task<bool> StartAlgorithm(SearchSpeeds searchSpeed, CancellationToken cancellationToken)
    {
        List<Node> unvisitedNodes = await GetAllNodes(_pathRunner.Grid);
        Node startNode = unvisitedNodes.Find(node => node.State == NodeState.Start) ?? throw new ArgumentException("No start node found");
        Node finishNode = unvisitedNodes.Find(node => node.State == NodeState.Finish) ?? throw new ArgumentException("No finish node found");

        startNode.GCost = 0;
        startNode.HCost = GetManhattanDistance(startNode, finishNode);
        finishNode.HCost = 0;

        List<Node> openSet = new()
        {
            startNode
        };

        SetMaxUpdateCount(searchSpeed);
        int updatedNodes = 0;
        while (openSet.Any())
        {
            cancellationToken.ThrowIfCancellationRequested();
            openSet.Sort(new AStarNodeComparer());
            Node currentNode = openSet.First();
            openSet.Remove(currentNode);
            currentNode.Visited = true;
            updatedNodes = await HandleVisitedChanged(updatedNodes);

            if (currentNode == finishNode)
            {
                await SendShortestPath(currentNode);
                return true;
            }

            await UpdateNeighbors(currentNode, finishNode, openSet);
        }
        return false;
    }

    private int GetManhattanDistance(Node a, Node b)
    {
        int xDistance = Math.Abs(a.X - b.X);
        int yDistance = Math.Abs(a.Y - b.Y);
        return xDistance + yDistance;
    }

    private async Task UpdateNeighbors(Node currentNode, Node finishNode, List<Node> openSet)
    {
        List<Node> neighbors = await GetNodeNeighbors(currentNode);
        await Task.Run(() =>
        {
            foreach (Node neighbor in neighbors)
            {
                if (neighbor.Visited || neighbor.State == NodeState.Wall)
                {
                    continue;
                }

                int tentativeGCost = currentNode.GCost + GetManhattanDistance(currentNode, neighbor);
                if (tentativeGCost <= neighbor.GCost)
                {
                    neighbor.PreviousNode = currentNode;
                    neighbor.GCost = tentativeGCost;
                    neighbor.HCost = GetManhattanDistance(neighbor, finishNode);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        });
    }
}
