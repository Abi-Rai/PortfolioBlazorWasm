using PortfolioBlazorWasm.Models.FactsApi;

namespace PortfolioBlazorWasm.Services.FactsApi;

public interface IFactsApiClient
{
    Task<int> GetCallsMadeFromSessionStorage();
    Task<FactDto> GetFactAsync();
    Task<FactDto> GetFactFromSessionStorage();
    Task<List<FactDto>> GetFactsFromSessionStorage();
}