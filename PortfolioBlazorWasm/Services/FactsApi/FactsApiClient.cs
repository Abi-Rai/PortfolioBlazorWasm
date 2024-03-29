﻿using PortfolioBlazorWasm.Models.FactsApi;
using PortfolioBlazorWasm.Services.SessionStorage;
using System.Net.Http.Json;

namespace PortfolioBlazorWasm.Services.FactsApi;
public class FactsApiClient : IFactsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ISessionStorageService _sessionStorageService;
    private readonly ILogger<FactsApiClient> _logger;
    private const string _factsKey = "facts";
    private const string _callsMadeKey = "callsMade";
    public FactsApiClient(HttpClient httpClient, ISessionStorageService sessionStorageService, ILogger<FactsApiClient> logger)
    {
        _httpClient = httpClient;
        _sessionStorageService = sessionStorageService;
        _logger = logger;
    }

    public async Task<FactDto> GetFactAsync()
    {
        try
        {
            if (await GetCallsMadeFromSessionStorage() >= 10)
            {
                return await GetFactFromSessionStorage();
            }

            var response = await _httpClient.GetAsync("random");
            await IncrementCallsMadeInSessionStorage();
            if (response.IsSuccessStatusCode)
            {
                var fact = await response.Content.ReadFromJsonAsync<FactDto>();
                if (fact is not null)
                {
                    await StoreFactInSessionStorage(fact);
                    return fact;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Problem calling api: {Message}", ex.Message);
            throw;
        }
        return await GetFactFromSessionStorage();
    }
    public async Task<List<FactDto>> GetFactsFromSessionStorage()
    {
        return await _sessionStorageService.GetValue(_factsKey, new List<FactDto>());
    }
    public async Task<int> GetCallsMadeFromSessionStorage()
    {
        return await _sessionStorageService.GetValue(_callsMadeKey, 0);
    }
    public async Task<FactDto> GetFactFromSessionStorage()
    {
        var facts = await GetFactsFromSessionStorage();
        var fact = facts[Random.Shared.Next(0, facts.Count)];
        return fact;
    }
    private async Task IncrementCallsMadeInSessionStorage()
    {
        var callsMade = await GetCallsMadeFromSessionStorage();
        callsMade++;
        await _sessionStorageService.SetValue(_callsMadeKey, callsMade);
    }

    private async Task StoreFactInSessionStorage(FactDto fact)
    {
        var facts = await GetFactsFromSessionStorage();
        facts.Add(fact);
        await _sessionStorageService.SetValue(_factsKey, facts);
    }
}