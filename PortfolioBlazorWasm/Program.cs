using Blazor.Analytics;
using FluentValidation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using PortfolioBlazorWasm.Models.TicTacToe.Settings;
using PortfolioBlazorWasm.Services.BankPa;
using PortfolioBlazorWasm.Services.CsvHelper;
using PortfolioBlazorWasm.Services.FactsApi;
using PortfolioBlazorWasm.Services.Pathfinding;
using PortfolioBlazorWasm.Services.SessionStorage;
using PortfolioBlazorWasm.Services.TicTacToe;
using Serilog;

namespace PortfolioBlazorWasm;

internal static class Program
{
    public static async Task Main(string[] args)
    {

        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        var configuration = builder.Configuration;
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        builder.Services.AddTransient<IPathfindingRunner, PathfindingRunner>();
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddScoped<IValidator<ChangeSettings>, SettingModelFluentValidator>();
        builder.Services.AddScoped<ISessionStorageService, SessionStorageService>();
        builder.Services.AddScoped<ICsvHelperService, CsvHelperService>();
        builder.Services.AddScoped<IBankPaService, BankPaService>();
        builder.Services.AddHttpClient<IFactsApiClient, FactsApiClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["factsapi"]!);
        });
        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            config.SnackbarConfiguration.PreventDuplicates = true;
            config.SnackbarConfiguration.NewestOnTop = true;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 700;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });
        builder.Services.AddMudBlazorDialog();

        builder.Services.AddGoogleAnalytics(configuration["GTAG_ID"]);
        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Error()
        .WriteTo.BrowserConsole()
        .CreateLogger();

        builder.Logging.AddSerilog();

        await builder.Build().RunAsync();
    }
}