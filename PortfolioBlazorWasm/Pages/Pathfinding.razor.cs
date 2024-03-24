using Microsoft.AspNetCore.Components;
using MudBlazor;
using PortfolioBlazorWasm.Components.Pathfinding;
using PortfolioBlazorWasm.Helpers.CustomExtensions;
using PortfolioBlazorWasm.Models.Pathfinding;
using PortfolioBlazorWasm.Models.Pathfinding.Enums;
using PortfolioBlazorWasm.Models.Pathfinding.Mazes;

namespace PortfolioBlazorWasm.Pages;

public partial class Pathfinding : IDisposable
{
    [CascadingParameter] public MudThemeProvider ThemeProvider { get; set; }
    [Inject] public ILogger<Pathfinding> Logger { get; set; }
    private bool _isAlgorithmRunning;
    private bool _isGridReset;
    private bool _isDragging;
    private bool _isPlaceWalls;
    
    private Node? _draggedNode;
    private SearchSettings _searchSettings;
    private CancellationTokenSource? _cts;
    private GridSettings _gridSettings;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _isAlgorithmRunning = false;
        _isGridReset = true;
        _isDragging = false;
        _isPlaceWalls = false;
        _searchSettings = new() { SearchSpeed = SearchSpeeds.Medium };
        _gridSettings = new GridSettings();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await PathFindingService.GenerateAndSetGridAsync(_gridSettings.RowCount, _gridSettings.ColumnCount);
            PathFindingService.VisitedChanged += OnNodesVisitedAsync;
            PathFindingService.ShortestFound += OnShortestPathFoundAsync;
            await InvokeAsync(StateHasChanged);
        }
    }

    private void OpenInstructionsDialog()
    {
        DialogOptions options = new()
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            DisableBackdropClick = false
        };
        DialogService.Show<Instructions>("Instructions Dialog", options);
    }

    #region Grid controls
    private async Task StartAlgorithm()
    {
        SnackBarService.AddTwoSecond($"Algorithm type:{_searchSettings.AlgorithmType} speed:{_searchSettings.SearchSpeed}", MudBlazor.Severity.Info);
        _isAlgorithmRunning = true;
        _isGridReset = false;
        _cts = new();
        try
        {
            var found = await PathFindingService.RunAlgorithm(_searchSettings, _cts.Token);
            if (found)
            {
                SnackBarService.AddTwoSecond("Shortest Path Found!", MudBlazor.Severity.Success);
            }
            else
            {
                SnackBarService.AddTwoSecond("No Path found!", MudBlazor.Severity.Warning);
            }
        }
        catch (OperationCanceledException)
        {
            SnackBarService.AddTwoSecond("Pathfinding cancelled", MudBlazor.Severity.Error);
            _cts.TryReset();
        }
        catch (Exception ex)
        {
            SnackBarService.AddTwoSecond(string.Format("Error occurred: {0}", ex.Message), MudBlazor.Severity.Error);
            Logger.LogError("Error occured: {message} stacktrace:{trace}", ex.Message, ex.StackTrace);
        }

        _isAlgorithmRunning = false;
    }
    private async Task GenerateMaze()
    {
        MazeTypes maze = MazeTypes.Kruskals;
        try
        {
            await PathFindingService.GenerateMaze(maze, _searchSettings.SearchSpeed);
        }
        catch (Exception ex)
        {
            SnackBarService.AddTwoSecond(string.Format("Error occured:{0}", ex.Message), MudBlazor.Severity.Error);
        }
        SnackBarService.AddTwoSecond($"Walls generated using {maze} algorithm!", MudBlazor.Severity.Info);
    }
    private async Task ResetGridToDefault()
    {
        _gridSettings = new();
        await PathFindingService.GenerateAndSetGridAsync(_gridSettings.RowCount, _gridSettings.ColumnCount);
        await ResetGridControls("Grid reset to default!");
    }
    private async Task RegenerateNewGrid()
    {
        await PathFindingService.GenerateAndSetGridAsync(_gridSettings.RowCount, _gridSettings.ColumnCount);
        await ResetGridControls("New matrix created!");
    }
    private async Task ClearWalls()
    {
        await PathFindingService.ClearGrid(true);
        await ResetGridControls("Walls & Nodes cleared!");
    }
    private async Task ClearVisitedNodes()
    {
        await PathFindingService.ClearGrid();
        await ResetGridControls("Nodes cleared!");
    }

    private Task ResetGridControls(string resetMesasge)
    {
        _isGridReset = true;
        _cts = new();
        SnackBarService.AddDuration(resetMesasge, MudBlazor.Severity.Info, 1500);
        return Task.CompletedTask;
    }

    private void CancelAlgorithm()
    {
        _cts?.Cancel();
    }

    private async Task OnShortestPathFoundAsync(object? sender, Stack<Node> shortestPath)
    {

        while (shortestPath.Any())
        {
            var node = shortestPath.Pop();
            if (node.State == NodeState.Start || node.State == NodeState.Finish)
            {
                continue;
            }

            node.State = NodeState.ShortestPath;
            await Task.Delay(1);
            await InvokeAsync(StateHasChanged);
        }
    }
    private async Task OnNodesVisitedAsync(object? sender, EventArgs args)
    {
        // needs to be a delay of at least 1 millisecond otherwise the re-rendering happens too fast.
        await Task.Delay(1);
        await InvokeAsync(StateHasChanged);
    }
    #endregion

    #region Drag & click cell methods
    private static void HandleWallPlace(Node nodeSelected)
    {
        if (nodeSelected.State == NodeState.None || nodeSelected.State == NodeState.Wall)
        {
            nodeSelected.State = ToggleWallState(nodeSelected.State);
        }
    }

    public static void HandleClickedCell(Node selectedNode)
    {
        if (selectedNode.State == NodeState.None || selectedNode.State == NodeState.Wall)
        {
            selectedNode.State = ToggleWallState(selectedNode.State);
        }
    }

    private static NodeState ToggleWallState(NodeState nodeState)
    {
        return (nodeState == NodeState.None ? NodeState.Wall : NodeState.None);
    }

    private string GetDragClass(NodeState nodeState)
    {
        if (_isPlaceWalls)
        {
            return (nodeState == NodeState.None || nodeState == NodeState.Wall) ? "draggable-node" : "not-allowed-node";
        }
        else
        {
            return IsDraggable(nodeState) ? "draggable-node" : "not-draggable-node";
        }
    }

    private string GetDropClass(NodeState nodeState)
    {
        if (nodeState == NodeState.None || (_draggedNode is not null && _draggedNode.State == nodeState))
            return "";
        return "not-droppable";
    }

    private string IsDroppableZone(NodeState nodeState)
    {
        return (nodeState == NodeState.None && _isDragging) ? "True" : "False";
    }

    private void HandleDragStart(Node dragStartNode)
    {
        if (dragStartNode.State == NodeState.Start || dragStartNode.State == NodeState.Finish)
        {
            _draggedNode = dragStartNode;
            _isDragging = true;
        }
    }

    private static string GetDraggable(NodeState nodeState)
    {
        if (IsDraggable(nodeState))
            return "true";
        return "false";
    }

    private static bool IsDraggable(NodeState nodeState)
    {
        return nodeState == NodeState.Start || nodeState == NodeState.Finish;
    }

    private void HandleDrop(Node dropZoneNode)
    {
        _isDragging = false;
        if (_draggedNode is null || dropZoneNode.State != NodeState.None)
        {
            return;
        }

        dropZoneNode.State = _draggedNode!.State;
        _draggedNode.State = NodeState.None;
    }
    #endregion

    #region Class styles
    private string GetClassVisited(bool visited)
    {
        if (visited)
        {
            if (!_isAlgorithmRunning)
            {
                return "visited-node-endingframe";
            }

            return _searchSettings.SearchSpeed switch
            {
                SearchSpeeds.Slow => "visited-node-slow",
                SearchSpeeds.Medium => "visited-node-medium",
                SearchSpeeds.Fast => "visited-node-fast",
                _ => throw new ArgumentException("Search speed not found")
            };
        }

        return "not-visited";
    }

    private string GetClass(NodeState nodeState)
    {
        return nodeState switch
        {
            NodeState.Start => "start-node",
            NodeState.Finish => "finish-node",
            NodeState.Wall => GetWallClass(),
            NodeState.ShortestPath => "shortest-path-node",
            NodeState.None => "none-node",
            _ => ""
        };
    }

    /// <summary>
    /// Since we don't want the walls to be animated unless placing walls.
    /// </summary>
    private string GetWallClass()
    {
        string className = ThemeProvider.IsDarkMode ? "wall-node-dark" : "wall-node-light";
        if (!_isAlgorithmRunning && _isPlaceWalls && _isGridReset)
        {
            className += "-animated";
        }
        return className;
    }

    #endregion

    #region Dispose pattern

    private bool _disposed = false;
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            PathFindingService.VisitedChanged -= OnNodesVisitedAsync;
            PathFindingService.ShortestFound -= OnShortestPathFoundAsync;
            _cts?.Dispose();
        }
        _disposed = true;
    }
    ~Pathfinding()
    {
        Dispose(false);
    }
    #endregion
}

