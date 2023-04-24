using PortfolioBlazorWasm.Models.Pathfinding;
using PortfolioBlazorWasm.Models.Pathfinding.Enums;
using PortfolioBlazorWasm.Models.Pathfinding.Mazes;

namespace PortfolioBlazorWasm.Services.Pathfinding.Algorithms.Mazes;

public class Kruskals : IMaze
{
    private readonly IPathfindingRunner _pathfindingRunner;
    private readonly int _rowCount;
    private readonly int _colCount;
    private readonly List<Edge> _edges;
    private readonly int[,] _parent;
    private int _maxUpdateCount;
    public Kruskals(IPathfindingRunner pathfindingRunner)
    {
        _pathfindingRunner = pathfindingRunner;
        _rowCount = _pathfindingRunner.Grid.GetLength(0);
        _colCount = _pathfindingRunner.Grid.GetLength(1);
        _edges = new List<Edge>();
        _parent = new int[_rowCount, _colCount];
    }

    public async Task GenerateMaze(SearchSpeeds searchSpeed)
    {
        SetMaxUpdateCount(searchSpeed);
        await Task.Run(() => ChangeNodeStateToWalls());
        int updatedNodes = 0;
        for (int i = 0; i < _rowCount; i++)
        {
            for (int j = 0; j < _colCount; j++)
            {
                if (i + 1 < _rowCount)
                    _edges.Add(new Edge(i * _colCount + j, (i + 1) * _colCount + j));
                if (j + 1 < _colCount)
                    _edges.Add(new Edge(i * _colCount + j, i * _colCount + j + 1));
                _parent[i, j] = i * _colCount + j;
            }
        }

        Random rand = new();
        for (int i = 0; i < _edges.Count; i++)
        {
            int r = rand.Next(i, _edges.Count);
            Edge temp = _edges[i];
            _edges[i] = _edges[r];
            _edges[r] = temp;
        }

        await Task.Run(async () =>
        {
            foreach (Edge edge in _edges)
            {
                int x1 = edge.X / _colCount;
                int y1 = edge.X % _colCount;
                int x2 = edge.Y / _colCount;
                int y2 = edge.Y % _colCount;

                int set1 = Find(edge.X);
                int set2 = Find(edge.Y);
                if (set1 != set2)
                {
                    Union(set1, set2);
                    if (x1 == x2)
                    {
                        if (_pathfindingRunner.Grid[x1, Math.Min(y1, y2) + 1].State != NodeState.Finish &&
                            _pathfindingRunner.Grid[x1, Math.Min(y1, y2) + 1].State != NodeState.Start)
                        {

                            _pathfindingRunner.Grid[x1, Math.Min(y1, y2) + 1].State = NodeState.None;
                            updatedNodes = await HandleNodeUpdates(updatedNodes);
                        }
                    }
                    else
                    {
                        if (_pathfindingRunner.Grid[Math.Min(x1, x2) + 1, y1].State != NodeState.Finish &&
                            _pathfindingRunner.Grid[Math.Min(x1, x2) + 1, y1].State != NodeState.Start)
                        {
                            _pathfindingRunner.Grid[Math.Min(x1, x2) + 1, y1].State = NodeState.None;
                            updatedNodes = await HandleNodeUpdates(updatedNodes);
                        }
                    }
                }
            }
        });
    }
    private async Task<int> HandleNodeUpdates(int updatedNodes)
    {
        if (++updatedNodes >= _maxUpdateCount)
        {
            await _pathfindingRunner.RaiseVisitedEvent(this, EventArgs.Empty);
            updatedNodes = 0;
        }
        return updatedNodes;
    }
    private Task ChangeNodeStateToWalls()
    {
        for (int i = 0; i < _pathfindingRunner.Grid.GetLength(0); i++)
        {
            for (int j = 0; j < _pathfindingRunner.Grid.GetLength(1); j++)
            {
                Node node = _pathfindingRunner.Grid[i, j];
                if (node.State is not NodeState.Start && node.State is not NodeState.Finish)
                {
                    _pathfindingRunner.Grid[i, j].State = NodeState.Wall;
                }
            }
        }
        return Task.CompletedTask;
    }

    private int Find(int i)
    {
        int x = i / _colCount;
        int y = i % _colCount;
        if (_parent[x, y] != i)
            return Find(_parent[x, y]);
        return i;
    }

    private void Union(int x, int y)
    {
        int setX = Find(x);
        int setY = Find(y);
        if (setX == setY) return;

        int parentXRow = setX / _colCount;
        int parentXCol = setX % _colCount;

        int parentYRow = setY / _colCount;
        int parentYCol = setY % _colCount;

        if (setX < setY)
            _parent[parentXRow, parentXCol] = setY;
        else
            _parent[parentYRow, parentYCol] = setX;
    }
    private void SetMaxUpdateCount(SearchSpeeds searchSpeed)
    {
        //_maxUpdateCount = searchSpeed switch
        //{
        //    SearchSpeeds.Slow => 1,
        //    SearchSpeeds.Medium => 50,
        //    SearchSpeeds.Fast => 100,
        //    _ => throw new ArgumentOutOfRangeException(nameof(searchSpeed))
        //};
        // TODO: Used for animating the walls
        _maxUpdateCount = 9999;
    }
}
