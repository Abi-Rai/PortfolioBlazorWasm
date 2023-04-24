using PortfolioBlazorWasm.Models.Pathfinding.Enums;

namespace PortfolioBlazorWasm.Services.Pathfinding.Algorithms;

public interface IAlgorithm
{
    Task<bool> StartAlgorithm(SearchSpeeds searchSpeed, CancellationToken cancellationToken);
}