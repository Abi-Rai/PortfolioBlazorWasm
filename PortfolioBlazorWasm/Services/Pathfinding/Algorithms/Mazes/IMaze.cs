using PortfolioBlazorWasm.Models.Pathfinding.Enums;

namespace PortfolioBlazorWasm.Services.Pathfinding.Algorithms.Mazes;

public interface IMaze
{
    Task GenerateMaze(SearchSpeeds searchSpeed);
}