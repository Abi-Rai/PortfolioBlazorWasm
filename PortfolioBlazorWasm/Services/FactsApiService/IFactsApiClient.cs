using PortfolioBlazorWasm.Models.FactsApi;

namespace PortfolioBlazorWasm.Services.FactsApiService;

public interface IFactsApiClient
{
    Task<FactDto> GetFactAsync();
}