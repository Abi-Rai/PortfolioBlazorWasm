using ApexCharts;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PortfolioBlazorWasm.Models.UkBankPa;
using PortfolioBlazorWasm.Services.BankPa;

namespace PortfolioBlazorWasm.Pages;

public partial class CsvToChart
{
    [Inject] public IBankPaService BankPaService { get; set; }

    private readonly DataGridFilterMode _filterMode = DataGridFilterMode.Simple;
    private List<BankRate>? _bankRates;
    private List<PersonalAllowance>? _personalAllowances;

    private ApexChartOptions<BankRate> _bankRateOptions;
    private ApexChartOptions<BankRate> _bankRatePercentOptions;

    private ApexChartOptions<PersonalAllowance> _personalAllowAmountOptions;
    private ApexChartOptions<PersonalAllowance> _personalAllowPercentOptions;
    private const string _bankRateChartId = "bankInterestRate";
    private const string _bankRatePercentChangeChartId = "bankRatePercentChange";
    private const string _personalAllowanceChartId = "personalAllowance";
    private const string _personalAllowancePercentChangeChartId = "personalAllowancePercentChange";
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var bankRateTask = BankPaService.GetBankRateRecords();
            var personalAllowTask = BankPaService.GetPersonalAllowanceRecords();
            List<Task> tasks = new()
            {
                bankRateTask,
                personalAllowTask
            };
            await Task.WhenAll(tasks);
            _bankRates = await bankRateTask;
            _personalAllowances = await personalAllowTask;
            if (_bankRates is not null && _personalAllowances is not null)
            {
                tasks.Clear();
                tasks.Add(SetBankRateChartOptions());
                tasks.Add(SetPersonalAllowanceChartOptions());
                await Task.WhenAll(tasks);
            }
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private Task SetBankRateChartOptions()
    {
        DateOnly selectionStart = _bankRates!.Last().DateChanged.AddYears(5);
        DateTimeOffset selectionStartDTO = new(selectionStart.Year, selectionStart.Month, selectionStart.Day, 0, 0, 0, TimeSpan.FromHours(10));
        _bankRatePercentOptions = new()
        {
            Theme = new Theme
            {
                Mode = Mode.Light,
                Palette = PaletteType.Palette2
            },
            Chart = new Chart()
            {
                Id = _bankRateChartId,
                Toolbar = new Toolbar
                {
                    AutoSelected = AutoSelected.Pan,
                    Show = true,
                    Tools = new()
                    {
                        Reset = false
                    }
                }
            }
        };
        _bankRateOptions = new()
        {
            Theme = new Theme
            {
                Mode = Mode.Light,
                Palette = PaletteType.Palette4
            },
            Xaxis = new XAxis
            {
                TickPlacement = TickPlacement.On
            },
            Chart = new Chart()
            {
                Id = _bankRatePercentChangeChartId,
                Toolbar = new Toolbar
                {
                    Show = true,
                    Tools = new()
                    {
                        Reset = false
                    }
                },
                Brush = new ApexCharts.Brush
                {
                    Enabled = true,
                    Target = _bankRateChartId
                },
                Selection = new Selection
                {
                    Enabled = true,
                    Xaxis = new SelectionXaxis
                    {
                        Min = selectionStartDTO.ToUnixTimeMilliseconds(),
                        Max = selectionStartDTO.AddYears(5).ToUnixTimeMilliseconds()
                    }
                }
            }
        };
        return Task.CompletedTask;
    }
    private Task SetPersonalAllowanceChartOptions()
    {
        DateOnly selectionStart = _personalAllowances!.Last().ToDate.AddYears(5);
        DateTimeOffset selectionStartDTO = new(selectionStart.Year, selectionStart.Month, selectionStart.Day, 0, 0, 0, TimeSpan.FromHours(10));
        _personalAllowPercentOptions = new()
        {
            Theme = new Theme
            {
                Mode = Mode.Light,
                Palette = PaletteType.Palette10
            },
            Chart = new Chart()
            {
                Id = _personalAllowanceChartId,
                Toolbar = new Toolbar
                {
                    AutoSelected = AutoSelected.Pan,
                    Show = true
                }
            },
            DataLabels = new ApexCharts.DataLabels
            {
                OffsetY = -6d
            }
        };
        _personalAllowAmountOptions = new()
        {
            Theme = new Theme
            {
                Mode = Mode.Light,
                Palette = PaletteType.Palette10
            },
            Xaxis = new XAxis
            {
                TickPlacement = TickPlacement.On
            },
            Chart = new Chart()
            {
                Id = _personalAllowancePercentChangeChartId,
                Toolbar = new Toolbar
                {
                    Show = true,
                    Tools = new()
                    {
                        Reset = false
                    }
                },
                Brush = new()
                {
                    Enabled = true,
                    Target = _personalAllowanceChartId
                },
                Selection = new Selection
                {
                    Enabled = true,
                    Xaxis = new SelectionXaxis
                    {
                        Min = selectionStartDTO.ToUnixTimeMilliseconds(),
                        Max = selectionStartDTO.AddYears(5).ToUnixTimeMilliseconds()
                    }
                }
            }

        };
        return Task.CompletedTask;
    }
}