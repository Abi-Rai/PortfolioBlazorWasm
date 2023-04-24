using Microsoft.AspNetCore.Components;
using PortfolioBlazorWasm.Models.FactsApi;
using PortfolioBlazorWasm.Services.FactsApi;
using PortfolioBlazorWasm.Services.SessionStorage;

namespace PortfolioBlazorWasm.Pages;

public partial class RandomFact
{
    [Inject] public IFactsApiClient FactsApiClient { get; set; }
    [Inject] public ISessionStorageService SessionStorageService { get; set; }

    private int _callsMade;
    private bool _overCallLimit { get => _callsMade >= 10; }

    private FactDto? _fact;
    private List<FactDto>? _sessionStoredFacts;
    private bool _fixedHeader = true;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            _sessionStoredFacts = await SessionStorageService.GetFactsFromSessionStorage();
            _callsMade = await SessionStorageService.GetCallsMadeFromSessionStorage();
            if (_sessionStoredFacts.Any())
            {
                _fact = _sessionStoredFacts.Last();
            }
            else
            {
                _fact = await FactsApiClient.GetFactAsync();
                _sessionStoredFacts = await SessionStorageService.GetFactsFromSessionStorage();
                _callsMade = await SessionStorageService.GetCallsMadeFromSessionStorage();
            }

            StateHasChanged();
        }
    }

    private async Task GetRandomFact()
    {
        _fact = await FactsApiClient.GetFactAsync();
        _sessionStoredFacts = await SessionStorageService.GetFactsFromSessionStorage();
        _callsMade = await SessionStorageService.GetCallsMadeFromSessionStorage();
    }

    private const string _programDICode = """
        builder.Services.AddSingleton<ISessionStorageService, SessionStorageService>();
        builder.Services.AddHttpClient<IFactsApiClient, FactsApiClient>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["factsapi"]!);
        });
        """;
    private const string _factRazorComponentCode = """
        public partial class RandomFact
        {
            [Inject] public IFactsApiClient FactsApiClient { get; }
            [Inject] public ISessionStorageService SessionStorageService { get; }

            private int _callsMade;
            private bool _overCallLimit { get => _callsMade >= 10; }

            private FactDto? _fact;
            private List<FactDto>? _sessionStoredFacts;
            private bool _fixedHeader = true;
            protected override async Task OnInitializedAsync()
            {
                await base.OnInitializedAsync();
            }

            protected override async Task OnAfterRenderAsync(bool firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);
                if (firstRender)
                {
                    _sessionStoredFacts = await SessionStorageService.GetFactsFromSessionStorage();
                    _callsMade = await SessionStorageService.GetCallsMadeFromSessionStorage();
                    if (_sessionStoredFacts.Any())
                    {
                        _fact = _sessionStoredFacts.Last();
                    }
                    else
                    {
                        _fact = await FactsApiClient.GetFactAsync();
                        _sessionStoredFacts = await SessionStorageService.GetFactsFromSessionStorage();
                        _callsMade = await SessionStorageService.GetCallsMadeFromSessionStorage();
                    }

                    StateHasChanged();
                }
            }

            private async Task GetRandomFact()
            {
                _fact = await FactsApiClient.GetFactAsync();
                _sessionStoredFacts = await SessionStorageService.GetFactsFromSessionStorage();
                _callsMade = await SessionStorageService.GetCallsMadeFromSessionStorage();
            }
        }
        """;
    private const string _factServiceCode = """
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
        """;
    private const string _sessionSeviceCode = """
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
        """;
}