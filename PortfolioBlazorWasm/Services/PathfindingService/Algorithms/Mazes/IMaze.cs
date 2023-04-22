using PortfolioBlazorWasm.Models.Pathfinding.Enums;

namespace PortfolioBlazorWasm.Services.PathfindingService.Algorithms.Mazes;

public interface IMaze
{
    Task GenerateMaze(SearchSpeeds searchSpeed);
}