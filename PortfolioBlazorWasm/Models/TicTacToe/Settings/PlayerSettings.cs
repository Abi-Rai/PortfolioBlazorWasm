using MudBlazor.Utilities;


namespace PortfolioBlazorWasm.Models.TicTacToe.Settings;

public sealed class PlayerBzrSettings : IEquatable<PlayerBzrSettings>
{
    public PlayerBzrSettings(string name, string marker, MudColor colorCell)
    {
        Name = name;
        Marker = marker;
        ColorCell = colorCell;

    }

    public string Name { get; set; }
    public string Marker { get; set; }
    public MudColor ColorCell { get; set; }


    public bool Equals(PlayerBzrSettings? other)
    {
        if (other is null) return false;
        return Name.ToLower() == other.Name.ToLower() && Marker.ToLower() == other.Marker.ToLower() && ColorCell.Value == other.ColorCell.Value;
    }
}
