using PortfolioBlazorWasm.Models.Pathfinding.Enums;

namespace PortfolioBlazorWasm.Services.PathfindingService.Algorithms;

public interface IAlgorithm
{
    Task<bool> StartAlgorithm(SearchSpeeds searchSpeed, CancellationToken cancellationToken);
}