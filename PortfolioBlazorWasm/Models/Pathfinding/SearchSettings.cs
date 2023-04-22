using PortfolioBlazorWasm.Models.Pathfinding.Enums;

namespace PortfolioBlazorWasm.Models.Pathfinding;

public class SearchSettings
{
    public AlgorithmTypes AlgorithmType { get; set; }
    public SearchSpeeds SearchSpeed { get; set; }
}
