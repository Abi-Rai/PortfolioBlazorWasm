using PortfolioBlazorWasm.Models.FactsApi;

namespace PortfolioBlazorWasm.Services.SessionStorage
{
    public interface ISessionStorageService
    {
        Task<int> GetCallsMadeFromSessionStorage();
        Task<FactDto> GetFactFromSessionStorage();
        Task<List<FactDto>> GetFactsFromSessionStorage();
        Task IncrementCallsMadeInSessionStorage();
        Task StoreFactInSessionStorage(FactDto fact);
    }
}