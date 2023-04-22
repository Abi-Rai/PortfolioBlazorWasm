namespace PortfolioBlazorWasm.Models.TicTacToe;

public class Player
{
    public string Name { get; private set; }
    public string Marker { get; private set; }
    public bool Human { get; private set; }


    private static int _playerNumberIncrementer = 1;
    public Player(string name, string marker, bool human = true)
    {
        Name = name;
        Marker = marker;
        Human = human;

    }
    public void ChangeName(string newName)
    {
        Name = newName;
    }
    public void ChangeMarker(string newMarker)
    {
        Marker = newMarker;
    }
    public void SetPlayerAsComputer()
    {
        Human = false;
        Name = "COMPUTER";
    }
    public void SetPlayerBackToHuman()
    {
        Human = true;
        Name = $"Reborn_{_playerNumberIncrementer}";
        IncrementPlayerNumber();
    }
    private static void IncrementPlayerNumber()
    {
        _playerNumberIncrementer++;
    }
}
