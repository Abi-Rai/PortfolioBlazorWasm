namespace PortfolioBlazorWasm.Models.TicTacToe;

public class PlayerBlazor : Player
{
    public string CellColorValue { get; private set; }
    public string TextColorValue { get; private set; }
    public PlayerBlazor(string name, string marker, string cellColorValue, string textColorValue = "white") : base(name, marker)
    {
        CellColorValue = cellColorValue;
        TextColorValue = textColorValue;
    }

    public void ChangeCellColorValue(string newCellColorValue)
    {
        CellColorValue = newCellColorValue;
    }
}
