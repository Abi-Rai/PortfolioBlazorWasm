using PortfolioBlazorWasm.Models.Pathfinding;
using PortfolioBlazorWasm.Models.Pathfinding.Enums;
using PortfolioBlazorWasm.Services.Pathfinding;

namespace PortfolioBlazorWasm.Services.Pathfinding.Algorithms;

public class Ddijkstras : BaseAlgorithm
{
    public Ddijkstras(IPathfindingRunner pathRunner) : base(pathRunner)
    {
    }
    public override async Task<bool> StartAlgorithm(SearchSpeeds searchSpeed, CancellationToken cancellationToken)
    {
        List<Node> unVisitedNodes = await GetAllNodes(_pathRunner.Grid);
        Node startNode = unVisitedNodes.Find(node => node.State == NodeState.Start) ?? throw new ArgumentException("No start node found");

        startNode.Distance = 0;
        SetMaxUpdateCount(searchSpeed);
        int updatedNodes = 0;
        while (unVisitedNodes.Any())
        {
            cancellationToken.ThrowIfCancellationRequested();
            unVisitedNodes.Sort();
            var closestNode = unVisitedNodes.First();
            unVisitedNodes.Remove(closestNode);

            if (closestNode.State is NodeState.Wall) continue;
            if (closestNode.Distance == int.MaxValue) return false;

            closestNode.Visited = true;
            updatedNodes = await HandleVisitedChanged(updatedNodes);

            if (closestNode.State == NodeState.Finish)
            {
                await SendShortestPath(closestNode);
                return true;
            }

            await UpdateNeighbors(closestNode);
        }
        return false;
    }

    private async Task UpdateNeighbors(Node closestNode)
    {
        var unvisitedNeighbors = await GetNodeNeighbors(closestNode);
        await Task.Run(() =>
        {
            for (int i = 0; i < unvisitedNeighbors.Count; ++i)
            {
                var neighbor = unvisitedNeighbors[i];
                neighbor.Distance = closestNode.Distance + 1;
                neighbor.PreviousNode = closestNode;
            }
        });
    }

}