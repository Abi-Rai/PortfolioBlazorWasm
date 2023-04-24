using PortfolioBlazorWasm.Models.TicTacToe;
using PortfolioBlazorWasm.Models.TicTacToe.Enums;
using PortfolioBlazorWasm.Models.TicTacToe.Settings;

namespace PortfolioBlazorWasm.Services.TicTacToe;

public class GameBoard
{
    private Cell[] _board;
    private PlayerNumber _startingPlayer;
    private PlayerNumber _playerNumberTurn;
    public bool PlayerTwoStarting => _startingPlayer == PlayerNumber.Two;
    public IReadOnlyList<Cell> Board { get => _board; }
    public int BoardSize { get; private set; }
    public RowSizes RowSize { get; private set; }
    public int RowLength { get; private set; }
    public PlayerBlazor PlayerOne { get; private set; }
    public PlayerBlazor PlayerTwo { get; private set; }
    public int TotalMoves { get; private set; }
    public List<Cell> WinningCells { get; private set; }
    public Dictionary<(int X, int Y), int> PointSelector { get; private set; }
    public bool PlayingAgainstComputer { get => !PlayerTwo.Human; }
    public bool IsWon { get; private set; }
    public bool IsDraw => TotalMoves > BoardSize && !IsWon;
    public bool GameOver => IsDraw || IsWon;
    public List<(string MoveLog, int CellIndex, string PlayerMarker)> MoveHistory { get; private set; }

    public GameBoard(
        RowSizes numberOfCells = RowSizes.Three,
        string playerOneName = "Player1",
        string playerTwoName = "Player2",
        string playerOneMarker = "X",
        string playerTwoMarker = "O",
        PlayerNumber startingPlayer = PlayerNumber.One)
    {
        RowSize = numberOfCells;
        BoardSize = (int)numberOfCells;
        RowLength = (int)Math.Sqrt(BoardSize);
        TotalMoves = 1;
        MoveHistory = new List<(string MoveLog, int CellIndex, string PlayerMarker)>();
        PlayerOne = new PlayerBlazor(playerOneName, playerOneMarker, "#bb53d5");
        PlayerTwo = new PlayerBlazor(playerTwoName, playerTwoMarker, "#6cd5a5");
        _startingPlayer = startingPlayer;
        _playerNumberTurn = _startingPlayer;
        PointSelector = new();
        WinningCells = new();
        IsWon = false;
        _board = NewGame();
    }
    public void ChangePlayerToComputer() => PlayerTwo.SetPlayerAsComputer();
    public void ChangePlayerToHuman() => PlayerTwo.SetPlayerBackToHuman();
    private PlayerBzrSettings GetPlayerSettings(PlayerBlazor player) => new(player.Name, player.Marker, player.CellColorValue);
    public bool IsCellWinningMove(int cellIndex) => WinningCells.Contains(GetCell(cellIndex));
    public Cell GetCell(int cellIndex) => _board[cellIndex];
    public PlayerBlazor GetCurrentPlayer() => _playerNumberTurn == PlayerNumber.One ? PlayerOne : PlayerTwo;
    public async Task ChangeGameSettings(ChangeSettings newSettings)
    {
        ChangeSettings currentSettings = await GetCurrentSettings();

        if (!currentSettings.PlayerOneSettings.Equals(newSettings.PlayerOneSettings))
        {
            PlayerOne.ChangeName(newSettings.PlayerOneSettings.Name);
            PlayerOne.ChangeMarker(newSettings.PlayerOneSettings.Marker);
            PlayerOne.ChangeCellColorValue(newSettings.PlayerOneSettings.ColorCell.Value);
        }

        if (!currentSettings.PlayerTwoSettings.Equals(newSettings.PlayerTwoSettings))
        {
            PlayerTwo.ChangeName(newSettings.PlayerTwoSettings.Name);
            PlayerTwo.ChangeMarker(newSettings.PlayerTwoSettings.Marker);
            PlayerTwo.ChangeCellColorValue(newSettings.PlayerTwoSettings.ColorCell.Value);
        }

        if (!currentSettings.BoardSettings.Equals(newSettings.BoardSettings))
        {
            _startingPlayer = newSettings.BoardSettings.StartingPlayer;
            _playerNumberTurn = _startingPlayer;
            if (currentSettings.BoardSettings.RowSizes != newSettings.BoardSettings.RowSizes)
            {
                await RestartGame(newSettings);
            }
        }
    }
    public async Task RestartGame(ChangeSettings? settings = null)
    {
        settings ??= await GetCurrentSettings();
        RowSize = settings.BoardSettings.RowSizes;
        BoardSize = (int)RowSize;
        RowLength = (int)Math.Sqrt(BoardSize);
        TotalMoves = 1;
        MoveHistory.Clear();
        PointSelector.Clear();
        WinningCells.Clear();
        _playerNumberTurn = settings.BoardSettings.StartingPlayer;
        IsWon = false;
        _board = NewGame();
    }

    public Task<ChangeSettings> GetCurrentSettings()
    {
        PlayerBzrSettings playerOneSettings = GetPlayerSettings(PlayerOne);
        PlayerBzrSettings playerTwoSettings = GetPlayerSettings(PlayerTwo);
        BoardSettings boardSettings = new(_startingPlayer, RowSize);
        ChangeSettings changeSettings = new(playerOneSettings, playerTwoSettings, boardSettings);
        return Task.FromResult(changeSettings);
    }

    private void CreatePointSelector()
    {
        PointSelector.Clear();
        int pointIndex = 0;
        for (int x = 0; x < RowLength; x++)
        {
            for (int y = 0; y < RowLength; y++)
            {
                PointSelector.Add((y, x), pointIndex++);
            }
        }
    }

    public string GetCellColorValue(int cellIndex) => GetCell(cellIndex).SetByPlayer switch
    {
        PlayerNumber.One => PlayerOne.CellColorValue,
        PlayerNumber.Two => PlayerTwo.CellColorValue,
        _ => throw new InvalidOperationException("should not be accessible unless set by a player")
    };
    public string GetTextColorValue(int cellIndex) => GetCell(cellIndex).SetByPlayer switch
    {
        PlayerNumber.One => PlayerOne.TextColorValue,
        PlayerNumber.Two => PlayerTwo.TextColorValue,
        _ => throw new InvalidOperationException("should not be accessible unless set by a player")
    };
    public Cell[] NewGame()
    {
        var emptyBoard = new Cell[BoardSize];
        for (int i = 0; i < emptyBoard.Length; i++)
        {
            emptyBoard[i] = new Cell(i.ToString(), i, false);
        }
        CreatePointSelector();
        return emptyBoard;
    }
    public void ChangeTurn()
    {
        _playerNumberTurn = _playerNumberTurn switch
        {
            PlayerNumber.One => PlayerNumber.Two,
            PlayerNumber.Two => PlayerNumber.One,
            _ => throw new ArgumentException("Error when changing turns, could not find enum"),
        };
    }

    public void MakeMove(int cellClicked)
    {
        Cell cell = _board[cellClicked];
        PlayerBlazor player = GetCurrentPlayer();
        LogMove(player, cell);
        cell.SetValue(player.Marker, _playerNumberTurn);
        _board[cell.Index] = cell;
    }
    public bool CheckWin()
    {
        if (CheckRows() || CheckColumns() || CheckDiagonals())
        {
            IsWon = true;
            return true;
        }
        WinningCells.Clear();
        return false;
    }
    private void LogMove(PlayerBlazor player, Cell cell)
    {
        string moveLog = $"Turn {TotalMoves}: {player.Name} set their marker:({player.Marker}) on cell:{cell.Index}";
        MoveHistory.Add((moveLog, cell.Index, player.Marker));
        TotalMoves++;
    }

    public void ComputerMakeMove()
    {
        Random rand = new();
        var availableCells = _board.Where(c => !c.IsPlayerSet).Select(c => c.Index).ToArray();
        int index = availableCells[rand.Next(0, availableCells.Length)];
        MakeMove(index);
    }
    private bool CheckDiagonals()
    {
        bool match1 = true;
        string marker1 = _board[0].ValueStr;
        WinningCells.Add(GetCell(0));
        for (int i = 1; i < RowLength; i++)
        {
            if (_board[i * RowLength + i].ValueStr != marker1)
            {
                match1 = false;
                WinningCells.Clear();
                break;
            }
            WinningCells.Add(GetCell(i * RowLength + i));
        }
        if (match1 && !string.IsNullOrEmpty(marker1))
        {
            return true;
        }
        bool match2 = true;
        string marker2 = _board[RowLength - 1].ValueStr;
        WinningCells.Add(GetCell(RowLength - 1));
        for (int i = 1; i < RowLength; i++)
        {
            if (_board[(i + 1) * (RowLength - 1)].ValueStr != marker2)
            {
                match2 = false;
                WinningCells.Clear();
                break;
            }
            WinningCells.Add(GetCell((i + 1) * (RowLength - 1)));
        }
        if (match2 && !string.IsNullOrEmpty(marker2))
        {
            return true;
        }

        return false;
    }
    private bool CheckColumns()
    {
        for (int i = 0; i < RowLength; i++)
        {
            bool match = true;
            string marker = _board[i].ValueStr;
            WinningCells.Add(GetCell(i));
            for (int j = 1; j < RowLength; j++)
            {
                if (_board[j * RowLength + i].ValueStr != marker)
                {
                    match = false;
                    WinningCells.Clear();
                    break;
                }
                WinningCells.Add(GetCell(j * RowLength + i));
            }
            if (match && !string.IsNullOrEmpty(marker))
            {
                return true;
            }
        }
        return false;
    }
    private bool CheckRows()
    {
        for (int i = 0; i < RowLength; i++)
        {
            bool match = true;
            string marker = _board[i * RowLength].ValueStr;
            WinningCells.Add(GetCell(i * RowLength));
            for (int j = 1; j < RowLength; j++)
            {
                if (_board[i * RowLength + j].ValueStr != marker)
                {
                    match = false;
                    WinningCells.Clear();
                    break;
                }
                WinningCells.Add(GetCell(i * RowLength + j));
            }
            if (match && !string.IsNullOrEmpty(marker))
            {
                return true;
            }
        }
        return false;
    }
}
