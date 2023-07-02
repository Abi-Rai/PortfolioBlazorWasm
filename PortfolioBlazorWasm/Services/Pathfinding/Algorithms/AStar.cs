using PortfolioBlazorWasm.Models.Pathfinding;
using PortfolioBlazorWasm.Models.Pathfinding.Enums;
using PortfolioBlazorWasm.Services.Pathfinding.Algorithms.Utils;

namespace PortfolioBlazorWasm.Services.Pathfinding.Algorithms;

public class AStar : BaseAlgorithm
{
    public AStar(IPathfindingRunner pathRunner) : base(pathRunner)
    {
    }
    public override async Task<bool> StartAlgorithm(SearchSpeeds searchSpeed, CancellationToken cancellationToken)
    {
        (Node startNode, Node finishNode) = GetStartAndFinishNodes(_pathRunner.Grid);

        startNode.GCost = 0;
        startNode.HCost = GetManhattanDistance(startNode, finishNode);
        finishNode.HCost = 0;

        List<Node> openSet = new()
        {
            startNode
        };
        AStarNodeComparer comparer = new();
        SetMaxUpdateCount(searchSpeed);
        int updatedNodes = 0;
        while (openSet.Any())
        {
            cancellationToken.ThrowIfCancellationRequested();
            openSet.Sort(comparer);
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
    private (Node startNode, Node finishNode) GetStartAndFinishNodes(Node[,] grid)
    {
        Node? startNode = null;
        Node? finishNode = null;

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Node node = grid[i, j];
                if (node.State == NodeState.Start)
                {
                    startNode = node;
                }
                else if (node.State == NodeState.Finish)
                {
                    finishNode = node;
                }

                if (startNode is not null && finishNode is not null)
                {
                    break;
                }
            }
        }

        if (startNode is null)
        {
            throw new ArgumentException("No start node found");
        }

        if (finishNode is null)
        {
            throw new ArgumentException("No finish node found");
        }

        return (startNode, finishNode);
    }
}
