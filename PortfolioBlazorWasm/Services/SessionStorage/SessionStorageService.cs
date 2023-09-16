using Microsoft.JSInterop;
using System.Text.Json;

namespace PortfolioBlazorWasm.Services.SessionStorage;

public class SessionStorageService : ISessionStorageService
{

    private readonly IJSRuntime _jsRuntime;

    public SessionStorageService(IJSRuntime jSRuntime)
    {
        _jsRuntime = jSRuntime;
    }

    public async Task<T> GetValue<T>(string key, T defaultValue)
    {
        var json = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", key);
        return json is null ? defaultValue : JsonSerializer.Deserialize<T>(json)!;
    }

    public async Task SetValue(string key, object value)
    {
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, JsonSerializer.Serialize(value));
    }
}
