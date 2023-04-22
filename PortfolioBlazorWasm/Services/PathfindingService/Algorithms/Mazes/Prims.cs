using PortfolioBlazorWasm.Models.Pathfinding;
using PortfolioBlazorWasm.Models.Pathfinding.Enums;

namespace PortfolioBlazorWasm.Services.PathfindingService.Algorithms.Mazes;

/// <summary>
/// Prims algorithm needs the grid to be an odd number, therefore it's not being implemented at the moment.
/// </summary>
public class Prims : IMaze
{
    private readonly IPathfindingRunner _pathRunner;

    public Prims(IPathfindingRunner pathRunner)
    {
        _pathRunner = pathRunner;
    }
    public Task GenerateMaze(SearchSpeeds searchSpeed)
    {
        for (int i = 0; i < _pathRunner.Grid.GetLength(0); i++)
        {
            for (int j = 0; j < _pathRunner.Grid.GetLength(1); j++)
            {
                Node node = _pathRunner.Grid[i, j];
                if (node.State is not NodeState.Start && node.State is not NodeState.Finish)
                {
                    _pathRunner.Grid[i, j].State = NodeState.Wall;
                }
            }
        }

        int startX = 1;
        int startY = 1;
        _pathRunner.Grid[startX, startY].State = NodeState.None;

        List<Node> walls = new();
        AddWalls(walls, startX, startY);

        while (walls.Any())
        {
            int randomIndex = new Random().Next(walls.Count);
            Node wall = walls[randomIndex];
            walls.RemoveAt(randomIndex);

            Node[] neighbors = GetNeighbors(wall.X, wall.Y);
            if (neighbors.Count(n => n.State == NodeState.None) == 1)
            {
                wall.State = NodeState.None;
                Node unvisited = neighbors.First(n => n.State == NodeState.Wall);
                unvisited.State = NodeState.None;
                AddWalls(walls, unvisited.X, unvisited.Y);
            }
        }
        return Task.CompletedTask;
    }

    private void AddWalls(List<Node> walls, int x, int y)
    {
        if (x > 0) walls.Add(_pathRunner.Grid[x - 1, y]);
        if (y > 0) walls.Add(_pathRunner.Grid[x, y - 1]);
        if (x < _pathRunner.Grid.GetLength(0) - 1) walls.Add(_pathRunner.Grid[x + 1, y]);
        if (y < _pathRunner.Grid.GetLength(1) - 1) walls.Add(_pathRunner.Grid[x, y + 1]);
    }

    private Node[] GetNeighbors(int x, int y)
    {
        List<Node> neighbors = new();
        if (x > 1) neighbors.Add(_pathRunner.Grid[x - 2, y]);
        if (y > 1) neighbors.Add(_pathRunner.Grid[x, y - 2]);
        if (x < _pathRunner.Grid.GetLength(0) - 2) neighbors.Add(_pathRunner.Grid[x + 2, y]);
        if (y < _pathRunner.Grid.GetLength(1) - 2) neighbors.Add(_pathRunner.Grid[x, y + 2]);
        return neighbors.ToArray();
    }
}
