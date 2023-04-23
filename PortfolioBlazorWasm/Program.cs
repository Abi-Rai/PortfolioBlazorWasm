using FluentValidation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using PortfolioBlazorWasm.Models.TicTacToe.Settings;
using PortfolioBlazorWasm.Services.BankPaService;
using PortfolioBlazorWasm.Services.CsvHelperService;
using PortfolioBlazorWasm.Services.FactsApiService;
using PortfolioBlazorWasm.Services.PathfindingService;
using PortfolioBlazorWasm.Services.SessionStorageService;
using PortfolioBlazorWasm.Services.TicTacToeService;

namespace PortfolioBlazorWasm
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<IValidator<ChangeSettings>, SettingModelFluentValidator>();
            builder.Services.AddScoped<IPathfindingRunner, PathfindingRunner>();
            builder.Services.AddScoped<ISessionStorageService, SessionStorageService>();
            builder.Services.AddScoped<ICsvHelperService, CsvHelperService>();
            builder.Services.AddScoped<IBankPaService, BankPaService>();
            builder.Services.AddHttpClient<IFactsApiClient, FactsApiClient>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["factsapi"]!);
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

            await builder.Build().RunAsync();
        }
    }
}