using PortfolioBlazorWasm.Models.TicTacToe.Enums;

namespace PortfolioBlazorWasm.Models.TicTacToe.Settings
{
    public sealed class BoardSettings : IEquatable<BoardSettings>
    {
        public BoardSettings(PlayerNumber startingPlayer, RowSizes rowSizes)
        {
            StartingPlayer = startingPlayer;
            RowSizes = rowSizes;
        }

        public PlayerNumber StartingPlayer { get; set; }
        public RowSizes RowSizes { get; set; }
        public bool Equals(BoardSettings? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return StartingPlayer == other.StartingPlayer &&
               RowSizes == other.RowSizes;
        }
        public override bool Equals(object? obj) => Equals(obj as BoardSettings);
        public override int GetHashCode()
        {
            return HashCode.Combine(StartingPlayer, RowSizes);
        }
    }
}
