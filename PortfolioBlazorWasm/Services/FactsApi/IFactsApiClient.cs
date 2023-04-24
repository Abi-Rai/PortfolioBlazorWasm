using PortfolioBlazorWasm.Models.FactsApi;

namespace PortfolioBlazorWasm.Services.FactsApi;

public interface IFactsApiClient
{
    Task<FactDto> GetFactAsync();
}