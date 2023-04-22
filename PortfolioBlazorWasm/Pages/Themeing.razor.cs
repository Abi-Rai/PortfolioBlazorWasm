namespace PortfolioBlazorWasm.Pages;

public partial class Themeing
{
    const string _mudThemeJSCode = """
    function watchDarkThemeMedia(dotNetHelper) {
        dotNetHelperTheme = dotNetHelper;
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function(e) {
            dotNetHelperTheme.invokeMethodAsync('SystemPreferenceChanged', window.matchMedia(""(prefers-color-scheme: dark)"").matches);
        });
    }
    """;
    const string _mudThemeRazorCode = """
    <MudThemeProvider @ref=""@_mudThemeProvider"" @bind-IsDarkMode=""@_isDarkMode""/>
    @code {
        private bool _isDarkMode;
        private MudThemeProvider _mudThemeProvider;
    
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isDarkMode = await _mudThemeProvider.GetSystemPreference();
                await _mudThemeProvider.WatchSystemPreference(OnSystemPreferenceChanged);
                StateHasChanged();
            }
        }
    
        private async Task OnSystemPreferenceChanged(bool newValue)
        {
            _isDarkMode = newValue;
            StateHasChanged();
        }
    }
    """;
    const string _mudThemeCascadingValue = """
        <MudMainContent>
            <CascadingValue Value="@_mudThemeProvider">
            @Body
            </CascadingValue>
        </MudMainContent>
        """;
    const string _mudThemeCascadingParameter = """
        [CascadingParameter] public MudThemeProvider ThemeProvider { get; set; }
        //
        private string GetClass(NodeState nodeState)
        {
            return nodeState switch
            {
                //
                NodeState.Wall => ThemeProvider.IsDarkMode?"wall-node-dark": "wall-node-light",
                //
            };
        }
        """;
    const string _highlightJSCode = """
    let dark = '/css/highlight/styles/monokai.min.css';
    let light = '/css/highlight/styles/vs.min.css';
    window.themeIsDark = function (isDark) {
        if (isDark) {
            document.getElementById(""theme"").href = dark;
        } else {
            document.getElementById(""theme"").href = light;
        }
    }
    window.highlightSnippet = function () {
        document.querySelectorAll('pre code').forEach((el) => {
            hljs.highlightBlock(el);
        });
    }
    """;
    const string _highlightRazorCode = """
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _isDarkMode = await _mudThemeProvider.GetSystemPreference();
            await _jsRunTime.InvokeAsync<Task>(""themeIsDark"", _isDarkMode);
            await _mudThemeProvider.WatchSystemPreference(OnSystemPreferenceChanged);
            StateHasChanged();
        }
    }
    private async Task OnToggleDarkMode(bool isDark)
    {
        _isDarkMode = isDark;
        await _jsRunTime.InvokeAsync<Task>(""themeIsDark"", _isDarkMode);
    }
    private async Task OnSystemPreferenceChanged(bool newValue)
    {
        await Task.Run(() => OnToggleDarkMode(newValue));
        StateHasChanged();
    }
    private void MudDrawerToggle(bool isOpen)
    {
        _mudDrawerOpen = isOpen;
    }
    """;
}