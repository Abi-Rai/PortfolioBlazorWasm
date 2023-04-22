using MudBlazor;

namespace PortfolioBlazorWasm.Helpers.CustomExtensions;

public static class SnackbarExtensions
{
    public static void AddTwoSecond(this ISnackbar snackbarService, string message, Severity severity = Severity.Normal)
    {
        snackbarService.Add(message, severity, config => { config.VisibleStateDuration = 2000; });
    }
    public static void AddDuration(this ISnackbar snackbarService, string message, Severity severity, int milliseconds)
    {
        snackbarService.Add(message, severity, config => { config.VisibleStateDuration = milliseconds; });
    }
}
