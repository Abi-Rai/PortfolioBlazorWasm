using PortfolioBlazorWasm.Models.Pathfinding;
using PortfolioBlazorWasm.Models.Pathfinding.Enums;
using PortfolioBlazorWasm.Models.Pathfinding.Mazes;
using PortfolioBlazorWasm.Services.PathfindingService.Algorithms;
using PortfolioBlazorWasm.Services.PathfindingService.Algorithms.Mazes;

namespace PortfolioBlazorWasm.Services.PathfindingService;

public class PathfindingRunner : IPathfindingRunner
{
    public Node[,] Grid { get; private set; }
    public event Func<object, EventArgs, Task> VisitedChanged;
    public event Func<object, Stack<Node>, Task> ShortestFound;

    private (int x, int y) _defaultStartPosition;
    private (int x, int y) _defaultFinishPosition;
    public IAlgorithm ChosenAlgorithm { get; private set; }
    public IMaze ChosenMaze { get; private set; }
    public async Task GenerateAndSetGridAsync(int row, int col)
    {
        await SetStartFinishPositions(row, col);
        Grid = await Task.Run(() => GenerateGrid(row, col));
    }

    private Task SetStartFinishPositions(int row, int col)
    {
        _defaultStartPosition = row == col ? (row / 2 - 1, row / 2 - 2) : (row / 2 - 1, row / 2);
        _defaultFinishPosition = (row / 2 - 1, col / 2);
        return Task.CompletedTask;
    }

    private Node[,] GenerateGrid(int row, int col)
    {
        Node[,] newGrid = new Node[row, col];
        for (int i = 0; i < newGrid.GetLength(0); i++)
        {
            for (int j = 0; j < newGrid.GetLength(1); j++)
            {
                newGrid[i, j] = new Node(i, j, int.MaxValue);
            }
        }
        newGrid[_defaultStartPosition.x, _defaultStartPosition.y].State = NodeState.Start;
        newGrid[_defaultFinishPosition.x, _defaultFinishPosition.y].State = NodeState.Finish;
        return newGrid;
    }
    public async Task GenerateMaze(MazeTypes mazeType, SearchSpeeds searchSpeed)
    {
        ChosenMaze = CreateMaze(mazeType);
        await ChosenMaze.GenerateMaze(searchSpeed);
    }
    public Task<bool> RunAlgorithm(SearchSettings searchSettings, CancellationToken cancellationToken)
    {
        ChosenAlgorithm = CreateAlgorithm(searchSettings.AlgorithmType);
        return ChosenAlgorithm.StartAlgorithm(searchSettings.SearchSpeed, cancellationToken);
    }
    protected virtual IMaze CreateMaze(MazeTypes mazeType)
    {
        return mazeType switch
        {
            MazeTypes.Kruskals => new Kruskals(this),
            _ => throw new ArgumentException("no maze type found", nameof(mazeType))
        };
    }
    protected virtual IAlgorithm CreateAlgorithm(AlgorithmTypes algorithmType)
    {
        return algorithmType switch
        {
            AlgorithmTypes.Ddijkstras => new Ddijkstras(this),
            AlgorithmTypes.AStar => new AStar(this),
            _ => throw new ArgumentException("no algorithm type found", nameof(algorithmType))
        };
    }
    public Task ClearGrid(bool shouldClearWalls = false)
    {
        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                Node node = Grid[i, j];
                node.Visited = false;
                if (node.State == NodeState.ShortestPath)
                {
                    node.State = NodeState.None;
                }
                if (shouldClearWalls && node.State == NodeState.Wall)
                {
                    node.State = NodeState.None;
                }
                node.Distance = int.MaxValue;
                node.GCost = int.MaxValue;
                node.HCost = 0;
            }
        }
        return Task.CompletedTask;
    }
    public Task RaiseVisitedEvent(object sender, EventArgs e)
    {
        return VisitedChanged.Invoke(sender, e);
    }
    public Task RaiseShortestFound(object sender, Stack<Node> e)
    {
        return ShortestFound.Invoke(sender, e);
    }
}
