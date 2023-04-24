using PortfolioBlazorWasm.Models.FactsApi;
using PortfolioBlazorWasm.Services.SessionStorage;
using System.Net.Http.Json;

namespace PortfolioBlazorWasm.Services.FactsApi;
public class FactsApiClient : IFactsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ISessionStorageService _sessionStorageService;

    public FactsApiClient(HttpClient httpClient, ISessionStorageService sessionStorageService)
    {
        _httpClient = httpClient;
        _sessionStorageService = sessionStorageService;
    }

    public async Task<FactDto> GetFactAsync()
    {
        try
        {
            if (await _sessionStorageService.GetCallsMadeFromSessionStorage() >= 10)
            {
                return await _sessionStorageService.GetFactFromSessionStorage();
            }

            var response = await _httpClient.GetAsync("random");
            await _sessionStorageService.IncrementCallsMadeInSessionStorage();
            if (response.IsSuccessStatusCode)
            {
                var fact = await response.Content.ReadFromJsonAsync<FactDto>();
                if (fact is not null)
                {
                    await _sessionStorageService.StoreFactInSessionStorage(fact);
                    return fact;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        return await _sessionStorageService.GetFactFromSessionStorage();
    }
}