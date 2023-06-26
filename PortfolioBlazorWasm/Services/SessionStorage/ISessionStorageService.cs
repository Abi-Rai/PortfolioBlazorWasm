namespace PortfolioBlazorWasm.Services.SessionStorage;

public interface ISessionStorageService
{
    public Task<T> GetValueFromSessionStorage<T>(string key, T defaultValue);
    public Task StoreValueInSessionStorage(string key, object value);
}