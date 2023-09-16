using Microsoft.AspNetCore.Components;
using PortfolioBlazorWasm.Models.FactsApi;
using PortfolioBlazorWasm.Services.FactsApi;

namespace PortfolioBlazorWasm.Pages;

public partial class RandomFact
{
    [Inject] public IFactsApiClient FactsService { get; set; }

    private int _callsMade;
    private bool OverCallLimit { get => _callsMade >= 10; }

    private FactDto? _fact;
    private List<FactDto>? _sessionStoredFacts;
    private bool _fixedHeader = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            _sessionStoredFacts = await FactsService.GetFactsFromSessionStorage();
            _callsMade = await FactsService.GetCallsMadeFromSessionStorage();
            if (_sessionStoredFacts.Any())
            {
                _fact = _sessionStoredFacts.Last();
            }
            else
            {
                _fact = await FactsService.GetFactAsync();
                _sessionStoredFacts = await FactsService.GetFactsFromSessionStorage();
                _callsMade = await FactsService.GetCallsMadeFromSessionStorage();
            }

            StateHasChanged();
        }
    }

    private async Task GetRandomFact()
    {
        _fact = await FactsService.GetFactAsync();
        _sessionStoredFacts = await FactsService.GetFactsFromSessionStorage();
        _callsMade = await FactsService.GetCallsMadeFromSessionStorage();
    }

    private const string _programDICode = """
        builder.Services.AddScoped<ISessionStorageService, SessionStorageService>();
        builder.Services.AddHttpClient<IFactsApiClient, FactsApiClient>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["factsapi"]!);
        });
        """;
    private const string _factRazorComponentCode = """
        public partial class RandomFact
        {
            [Inject] public IFactsApiClient FactsService { get; set; }

            private int _callsMade;
            private bool OverCallLimit { get => _callsMade >= 10; }

            private FactDto? _fact;
            private List<FactDto>? _sessionStoredFacts;
            private bool _fixedHeader = true;

            protected override async Task OnAfterRenderAsync(bool firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);
                if (firstRender)
                {
                    _sessionStoredFacts = await FactsService.GetFactsFromSessionStorage();
                    _callsMade = await FactsService.GetCallsMadeFromSessionStorage();
                    if (_sessionStoredFacts.Any())
                    {
                        _fact = _sessionStoredFacts.Last();
                    }
                    else
                    {
                        _fact = await FactsService.GetFactAsync();
                        _sessionStoredFacts = await FactsService.GetFactsFromSessionStorage();
                        _callsMade = await FactsService.GetCallsMadeFromSessionStorage();
                    }

                    StateHasChanged();
                }
            }

            private async Task GetRandomFact()
            {
                _fact = await FactsService.GetFactAsync();
                _sessionStoredFacts = await FactsService.GetFactsFromSessionStorage();
                _callsMade = await FactsService.GetCallsMadeFromSessionStorage();
            }
        }
        """;
    private const string _factServiceCode = """
        public class FactsApiClient : IFactsApiClient
        {
            private readonly HttpClient _httpClient;
            private readonly ISessionStorageService _sessionStorageService;
            private const string _factsKey = "facts";
            private const string _callsMadeKey = "callsMade";
            public FactsApiClient(HttpClient httpClient, ISessionStorageService sessionStorageService)
            {
                _httpClient = httpClient;
                _sessionStorageService = sessionStorageService;
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
                    Console.WriteLine(ex.Message);
                    throw;
                }
                return await GetFactFromSessionStorage();
            }
            public async Task<List<FactDto>> GetFactsFromSessionStorage()
            {
                return await _sessionStorageService.GetValueFromSessionStorage(_factsKey, new List<FactDto>());
            }
            public async Task<int> GetCallsMadeFromSessionStorage()
            {
                return await _sessionStorageService.GetValueFromSessionStorage(_callsMadeKey, 0);
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
                await _sessionStorageService.StoreValueInSessionStorage(_callsMadeKey, callsMade);
            }

            private async Task StoreFactInSessionStorage(FactDto fact)
            {
                var facts = await GetFactsFromSessionStorage();
                facts.Add(fact);
                await _sessionStorageService.StoreValueInSessionStorage(_factsKey, facts);
            }
        }
        """;
    private const string _sessionSeviceCode = """
        public class SessionStorageService : ISessionStorageService
        {

            private readonly IJSRuntime _jsRuntime;

            public SessionStorageService(IJSRuntime jSRuntime)
            {
                _jsRuntime = jSRuntime;
            }

            public async Task<T> GetValueFromSessionStorage<T>(string key, T defaultValue)
            {
                var json = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", key);
                return json is null ? defaultValue : JsonSerializer.Deserialize<T>(json)!;
            }

            public async Task StoreValueInSessionStorage(string key, object value)
            {
                await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, JsonSerializer.Serialize(value));
            }
        }
        """;
}