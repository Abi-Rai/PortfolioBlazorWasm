using PortfolioBlazorWasm.Models.Pathfinding;
using PortfolioBlazorWasm.Models.Pathfinding.Enums;
using PortfolioBlazorWasm.Models.Pathfinding.Mazes;
using PortfolioBlazorWasm.Services.PathfindingService.Algorithms;
using PortfolioBlazorWasm.Services.PathfindingService.Algorithms.Mazes;

namespace PortfolioBlazorWasm.Services.PathfindingService;

public interface IPathfindingRunner
{
    Node[,] Grid { get; }
    IAlgorithm ChosenAlgorithm { get; }
    IMaze ChosenMaze { get; }

    event Func<object, Stack<Node>, Task> ShortestFound;
    event Func<object, EventArgs, Task> VisitedChanged;

    Task ClearGrid(bool shouldClearWalls = false);

    Task GenerateAndSetGridAsync(int row, int col);
    Task GenerateMaze(MazeTypes mazeType, SearchSpeeds searchSpeed);
    Task RaiseShortestFound(object sender, Stack<Node> e);
    Task RaiseVisitedEvent(object sender, EventArgs e);
    Task<bool> RunAlgorithm(SearchSettings searchSettings, CancellationToken cancellationToken);
}