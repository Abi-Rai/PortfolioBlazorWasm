using Microsoft.JSInterop;
using PortfolioBlazorWasm.Models.FactsApi;
using System.Text.Json;

namespace PortfolioBlazorWasm.Services.SessionStorage;

public class SessionStorageService : ISessionStorageService
{
    private const string _factsKey = "facts";
    private const string _callsMadeKey = "callsMade";
    private readonly IJSRuntime _jsRuntime;

    public SessionStorageService(IJSRuntime jSRuntime)
    {
        _jsRuntime = jSRuntime;
    }
    public async Task<List<FactDto>> GetFactsFromSessionStorage()
    {
        return await GetValueFromSessionStorage(_factsKey, new List<FactDto>());
    }
    public async Task<int> GetCallsMadeFromSessionStorage()
    {
        return await GetValueFromSessionStorage(_callsMadeKey, 0);
    }

    public async Task<FactDto> GetFactFromSessionStorage()
    {
        var facts = await GetFactsFromSessionStorage();
        var fact = facts[Random.Shared.Next(0, facts.Count)];
        return fact;
    }
    public async Task IncrementCallsMadeInSessionStorage()
    {
        var callsMade = await GetCallsMadeFromSessionStorage();
        callsMade++;
        await StoreValueInSessionStorage(_callsMadeKey, callsMade);
    }

    public async Task StoreFactInSessionStorage(FactDto fact)
    {
        var facts = await GetFactsFromSessionStorage();
        facts.Add(fact);
        await StoreValueInSessionStorage(_factsKey, facts);
    }

    private async Task<T> GetValueFromSessionStorage<T>(string key, T defaultValue)
    {
        var json = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", key);
        return json is null ? defaultValue : JsonSerializer.Deserialize<T>(json)!;
    }

    private async Task StoreValueInSessionStorage(string key, object value)
    {
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, JsonSerializer.Serialize(value));
    }
}
