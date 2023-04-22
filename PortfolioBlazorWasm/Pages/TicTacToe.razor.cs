using MudBlazor;
using PortfolioBlazorWasm.Components.TicTacToe;
using PortfolioBlazorWasm.Models.TicTacToe.Settings;
using PortfolioBlazorWasm.Services.TicTacToeService;

namespace PortfolioBlazorWasm.Pages;

public partial class TicTacToe
{
    //Game logic
    private GameBoard _gameBoard { get; set; }
    private bool _playWithComputer { get; set; }

    //Component loaders
    private bool _showHistory { get; set; }
    private bool _gameNotStarted { get; set; }

    //constant css styles
    private const string _winningCellStyle = "color: var(--mud-palette-success-text); background-color:var(--mud-palette-success)";
    private const string _defaultCellStyle = "color: var(--mud-palette-text-secondary)";
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _playWithComputer = false;
        _showHistory = true;
        _gameNotStarted = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            _gameBoard = new();
            StateHasChanged();
        }
    }

    private void ValidateAndMakeMove(int cellClicked)
    {
        HandleGameStart();
        if (!CanMakeMove(cellClicked))
            return;
        RemoveSnackbar();
        _gameBoard.MakeMove(cellClicked);
        _gameBoard.CheckWin();
        if (!_gameBoard.GameOver)
        {
            _gameBoard.ChangeTurn();
            if (_playWithComputer)
                MakeComputerMove();
        }
    }

    private void MakeComputerMove()
    {
        if (!_gameBoard.GetCurrentPlayer().Human)
        {
            _gameBoard.ComputerMakeMove();
            _gameBoard.CheckWin();
            if (!_gameBoard.GameOver)
            {
                _gameBoard.ChangeTurn();
            }
        }
    }

    private bool CanMakeMove(int cellClicked)
    {
        if (_gameBoard.GameOver)
        {
            SnackBarService.Add("game has ended, restart game to play again", MudBlazor.Severity.Info);
        }
        else if (_gameBoard.GetCell(cellClicked).IsPlayerSet)
        {
            SnackBarService.Add("cell already marked", MudBlazor.Severity.Warning);
        }
        else
        {
            return true;
        }

        return false;
    }

    private void HandleGameStart()
    {
        if (_gameNotStarted)
        {
            if (_playWithComputer && !_gameBoard.PlayingAgainstComputer)
            {
                _gameBoard.ChangePlayerToComputer();
            }
            else if (!_playWithComputer && _gameBoard.PlayingAgainstComputer)
            {
                _gameBoard.ChangePlayerToHuman();
            }

            _gameNotStarted = false;
        }
    }

    private async Task ShowSettings()
    {
        var currentSettings = await _gameBoard.GetCurrentSettings();
        var paramteters = new DialogParameters
        {
            ["GameSettings"] = currentSettings
        };
        var dialog = await DialogService.ShowAsync<ChangePlayerSettings>("Game Settings", paramteters);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var returnedValue = await dialog.GetReturnValueAsync<ChangeSettings>();
            try
            {
                await _gameBoard.ChangeGameSettings(returnedValue);
                SnackBarService.Add("Changed settings successfully", MudBlazor.Severity.Success);
            }
            catch (Exception ex)
            {
                SnackBarService.Add($"Error changing settings:{ex.Message}", MudBlazor.Severity.Error, config =>
                {
                    config.VisibleStateDuration = 2000;
                });
            }
        }
        else
        {
            SnackBarService.Add("Cancelled");
        }
    }

    private string GetGameState()
    {
        if (_gameBoard.IsDraw)
        {
            return "Game is a Draw";
        }

        if (_gameBoard.IsWon)
        {
            return $"{_gameBoard.GetCurrentPlayer().Name} Won";
        }

        return "Turn";
    }

    private void RemoveSnackbar()
    {
        SnackBarService.Clear();
        StateHasChanged();
    }

    private async Task ResetGame()
    {
        RemoveSnackbar();
        var currentSettings = await _gameBoard.GetCurrentSettings();
        await _gameBoard.RestartGame(currentSettings);
        _gameNotStarted = true;
    }

    private string GetCursorStyle(int cellIndex)
    {
        if (_gameBoard.GameOver)
        {
            return "";
        }
        else
        {
            return _gameBoard.GetCell(cellIndex).IsPlayerSet ? "cursor-not-allowed" : "cursor-pointer grow-cell";
        }
    }

    private string GetCellStyle(int cellIndex)
    {
        if (_gameBoard.IsWon)
        {
            return _gameBoard.IsCellWinningMove(cellIndex) ? _winningCellStyle : _defaultCellStyle;
        }
        else if (_gameBoard.GetCell(cellIndex).IsPlayerSet)
        {
            return $"color:{_gameBoard.GetTextColorValue(cellIndex)}; background-color:{_gameBoard.GetCellColorValue(cellIndex)}";
        }

        return _defaultCellStyle;
    }

    private Color GetTimeLineColor(int currentIndex) => currentIndex == _gameBoard.MoveHistory.Count - 1 ? Color.Secondary : Color.Primary;
    private Color GetTimeLineTextColor(int turnIndex)
    {
        if (_gameBoard.IsWon)
            return _gameBoard.IsCellWinningMove(turnIndex) ? Color.Success : Color.Primary;
        return Color.Inherit;
    }
}