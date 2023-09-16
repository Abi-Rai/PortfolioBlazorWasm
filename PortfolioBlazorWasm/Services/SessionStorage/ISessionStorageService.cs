namespace PortfolioBlazorWasm.Services.SessionStorage;

public interface ISessionStorageService
{
    public Task<T> GetValue<T>(string key, T defaultValue);
    public Task SetValue(string key, object value);
}