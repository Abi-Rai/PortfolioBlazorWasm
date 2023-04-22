using PortfolioBlazorWasm.Models.TicTacToe.Enums;

namespace PortfolioBlazorWasm.Models.TicTacToe;

public struct Cell : IEquatable<Cell>
{

    public string ValueStr { get; private set; }
    public bool IsPlayerSet { get; set; }
    public int Index { get; set; }
    public PlayerNumber? SetByPlayer { get; set; } = null;
    public Cell(string valueStr, int index, bool isPlayerSet)
    {
        ValueStr = valueStr;
        Index = index;
        IsPlayerSet = isPlayerSet;
    }
    public void SetValue(string valueStr, PlayerNumber playerSet)
    {
        CellSetInfo(playerSet);
        ValueStr = valueStr;
    }

    private void CellSetInfo(PlayerNumber playerSet)
    {
        IsPlayerSet = true;
        SetByPlayer = playerSet;
    }

    public bool Equals(Cell other)
    {
        return ValueStr == other.ValueStr && Index == other.Index;
    }

    public override bool Equals(object? obj)
    {
        return obj is Cell cell && Equals(cell);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ValueStr, Index);
    }

    public static bool operator ==(Cell left, Cell right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Cell left, Cell right)
    {
        return !(left == right);
    }
}
