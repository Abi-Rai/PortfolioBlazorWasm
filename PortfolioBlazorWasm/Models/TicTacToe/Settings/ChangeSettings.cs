namespace PortfolioBlazorWasm.Models.TicTacToe.Settings
{
    public sealed class ChangeSettings : IEquatable<ChangeSettings>
    {
        public ChangeSettings(PlayerBzrSettings playerOneSettings, PlayerBzrSettings playerTwoSettings, BoardSettings boardSettings)
        {
            PlayerOneSettings = playerOneSettings;
            PlayerTwoSettings = playerTwoSettings;
            BoardSettings = boardSettings;
        }

        public PlayerBzrSettings PlayerOneSettings { get; set; }
        public PlayerBzrSettings PlayerTwoSettings { get; set; }
        public BoardSettings BoardSettings { get; set; }

        public bool Equals(ChangeSettings? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(this, other);
        }

    }
}
