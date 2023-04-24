using PortfolioBlazorWasm.Models.Pathfinding;
using PortfolioBlazorWasm.Models.Pathfinding.Enums;
using PortfolioBlazorWasm.Services.Pathfinding;

namespace PortfolioBlazorWasm.Services.Pathfinding.Algorithms;

public abstract class BaseAlgorithm : IAlgorithm
{
    protected readonly IPathfindingRunner _pathRunner;
    protected int _maxUpdateCount;
    protected BaseAlgorithm(IPathfindingRunner pathRunner)
    {
        _pathRunner = pathRunner;
    }

    public abstract Task<bool> StartAlgorithm(SearchSpeeds searchSpeed, CancellationToken cancellationToken);
    protected async Task<List<Node>> GetAllNodes(Node[,] matrix)
    {
        List<Node> result = new();
        return await Task.Run(() =>
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    result.Add(matrix[i, j]);
                }
            }
            return result;
        });
    }

    protected async Task<int> HandleVisitedChanged(int updatedNodes)
    {
        if (++updatedNodes >= _maxUpdateCount)
        {
            await _pathRunner.RaiseVisitedEvent(this, EventArgs.Empty);
            updatedNodes = 0;
        }
        return updatedNodes;
    }

    protected async Task SendShortestPath(Node closestNode)
    {
        Stack<Node>? path = new();
        var currentNode = closestNode;
        while (currentNode is not null)
        {
            path.Push(currentNode);
            currentNode = currentNode.PreviousNode;
        }
        await _pathRunner.RaiseShortestFound(this, path);
    }
    protected void SetMaxUpdateCount(SearchSpeeds searchSpeed) => _maxUpdateCount = searchSpeed switch
    {
        SearchSpeeds.Slow => 1,
        SearchSpeeds.Medium => 10,
        SearchSpeeds.Fast => 50,
        _ => throw new ArgumentOutOfRangeException(nameof(searchSpeed))
    };

    protected async Task<List<Node>> GetNodeNeighbors(Node closestNode)
    {
        return await Task.Run(() =>
        {
            var neighbors = new List<Node>();
            (int x, int y) = (closestNode.X, closestNode.Y);
            if (x > 0) neighbors.Add(_pathRunner.Grid[x - 1, y]);
            if (x < _pathRunner.Grid.GetLength(0) - 1) neighbors.Add(_pathRunner.Grid[x + 1, y]);
            if (y > 0) neighbors.Add(_pathRunner.Grid[x, y - 1]);
            if (y < _pathRunner.Grid.GetLength(1) - 1) neighbors.Add(_pathRunner.Grid[x, y + 1]);
            return neighbors.Where(x => !x.Visited).ToList();
        });
    }

}