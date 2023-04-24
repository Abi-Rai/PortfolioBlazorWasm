using FluentAssertions;
using Moq;
using PortfolioBlazorWasm.Models.TicTacToe;
using PortfolioBlazorWasm.Models.TicTacToe.Enums;
using PortfolioBlazorWasm.Models.TicTacToe.Settings;
using PortfolioBlazorWasm.Services.TicTacToe;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace PortfolioBlazorWasm.Tests.Models.TicTacToe
{
    public class GameBoardTests
    {
        private readonly MockRepository _mockRepository;
        private readonly ITestOutputHelper _output;
        public GameBoardTests(ITestOutputHelper output)
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _output = output;
        }

        private GameBoard CreateGameBoard(RowSizes numberOfCells = RowSizes.Three,
        string playerOneName = "Player1",
        string playerTwoName = "Player2",
        string playerOneMarker = "X",
        string playerTwoMarker = "O",
        PlayerNumber startingPlayer = PlayerNumber.One)
        {
            return new GameBoard(numberOfCells, playerOneName, playerTwoName, playerOneMarker, playerTwoMarker, startingPlayer);
        }
        private void MakeMovesOnBoard(GameBoard gameBoard, List<int> moveSequence)
        {
            foreach (var move in moveSequence.Take(moveSequence.Count - 1))
            {
                gameBoard.MakeMove(move);
                gameBoard.CheckWin().Should().BeFalse();
                gameBoard.ChangeTurn();
            }
            gameBoard.MakeMove(moveSequence.Last());
        }

        [Fact]
        public async Task ChangeGameSettings_WhenGivenNewSettings_ShouldChangeTheRespectiveValues()
        {
            // Arrange
            string oneName = "John", oneMarker = "Y", oneCell = "#ffd254";
            string twoName = "Tom", twoMarker = "Z", twoCell = "#5d87e7";
            var gameBoard = CreateGameBoard();
            PlayerBzrSettings playerOneSettings = new(oneName, oneMarker, oneCell);
            PlayerBzrSettings playerTwoSettings = new(twoName, twoMarker, twoCell);
            BoardSettings boardSettings = new(PlayerNumber.Two, RowSizes.Four);
            PlayerBlazor playerOneChanged = new(oneName, oneMarker, playerOneSettings.ColorCell.Value);
            PlayerBlazor playerTwoChanged = new(twoName, twoMarker, playerTwoSettings.ColorCell.Value);

            ChangeSettings newSettings = new(playerOneSettings, playerTwoSettings, boardSettings);
            // Act
            await gameBoard.ChangeGameSettings(newSettings);

            // Assert
            gameBoard.PlayerOne.Should().BeEquivalentTo<PlayerBlazor>(playerOneChanged);
            gameBoard.PlayerTwo.Should().BeEquivalentTo<PlayerBlazor>(playerTwoChanged);
        }

        [Fact]
        public async Task RestartGame_WhenNotGivenNewSettings_ShouldRestartGameWithDefaultSettings()
        {
            // Arrange
            var gameBoard = CreateGameBoard();

            // Act
            await gameBoard.RestartGame();

            // Assert
            gameBoard.Should().BeEquivalentTo(gameBoard);
        }

        [Fact]
        public async Task RestartGame_WhenCreatedWithDifferentRowSizeBoard_ShouldRestartGameWithNewBoard()
        {
            // Arrange
            var gameBoard = CreateGameBoard(RowSizes.Four);
            
            // Act
            await gameBoard.RestartGame();

            // Assert
            gameBoard.Should().BeEquivalentTo(gameBoard);
        }

        [Fact]
        public void ChangePlayerToComputer_ShouldChangePlayerTwoToNotHuman()
        {
            // Arrange
            var gameBoard = CreateGameBoard();

            // Act
            gameBoard.ChangePlayerToComputer();

            // Assert
            gameBoard.PlayerTwo.Human.Should().BeFalse();
            gameBoard.PlayingAgainstComputer.Should().BeTrue();
        }

        [Fact]
        public void ChangePlayerToHuman_ShouldChangePlayerTwoToHuman()
        {
            // Arrange
            var gameBoard = CreateGameBoard();

            // Act
            gameBoard.ChangePlayerToHuman();

            // Assert
            gameBoard.PlayerTwo.Human.Should().BeTrue();
            gameBoard.PlayingAgainstComputer.Should().BeFalse();
        }

        [Fact]
        public async Task GetCurrentSettings_WhenDefaultSettings_ShouldReturnDefaultSettings()
        {
            // Arrange
            var gameBoard = CreateGameBoard();
            PlayerBzrSettings playerOneSettings = new(gameBoard.PlayerOne.Name, gameBoard.PlayerOne.Marker, gameBoard.PlayerOne.CellColorValue);
            PlayerBzrSettings playerTwoSettings = new(gameBoard.PlayerTwo.Name, gameBoard.PlayerTwo.Marker, gameBoard.PlayerTwo.CellColorValue);
            BoardSettings boardSettings = new(PlayerNumber.One, RowSizes.Three);
            ChangeSettings expectedSettings = new(playerOneSettings, playerTwoSettings, boardSettings);

            // Act
            var result = await gameBoard.GetCurrentSettings();

            // Assert
            result.Should().BeEquivalentTo(expectedSettings);
        }

        [Fact]
        public void GetCellColorValue_ShouldReturnCorrectColorValue()
        {
            // Arrange
            var gameBoard = CreateGameBoard();
            int firstMoveIndex = 0;
            int secondMoveIndex = 1;


            // Act
            gameBoard.MakeMove(firstMoveIndex);
            gameBoard.ChangeTurn();
            gameBoard.MakeMove(secondMoveIndex);
            var firstResult = gameBoard.GetCellColorValue(firstMoveIndex);
            var secondResult = gameBoard.GetCellColorValue(secondMoveIndex);

            // Assert
            firstResult.Should().Be(gameBoard.PlayerOne.CellColorValue);
            secondResult.Should().Be(gameBoard.PlayerTwo.CellColorValue);
        }
        [Fact]
        public void GetCellColorValue_GettingCellColorWithoutPlayerSet_ShouldThrowException()
        {
            // Arrange
            var gameBoard = CreateGameBoard();

            // Act
            Action act = () => gameBoard.GetCellColorValue(0);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GetTextColorValue_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var gameBoard = CreateGameBoard();
            int firstMoveIndex = 0;
            int secondMoveIndex = 1;


            // Act
            gameBoard.MakeMove(firstMoveIndex);
            gameBoard.ChangeTurn();
            gameBoard.MakeMove(secondMoveIndex);
            var firstResult = gameBoard.GetTextColorValue(firstMoveIndex);
            var secondResult = gameBoard.GetTextColorValue(secondMoveIndex);

            // Assert
            firstResult.Should().Be(gameBoard.PlayerOne.TextColorValue);
            secondResult.Should().Be(gameBoard.PlayerTwo.TextColorValue);
        }

        [Fact]
        public void GetTextColorValue_GettingCellColorWithoutPlayerSet_ShouldThrowException()
        {
            // Arrange
            var gameBoard = CreateGameBoard();

            // Act
            Action act = () => gameBoard.GetTextColorValue(0);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ChangeTurn_ShouldChangePlayerTurn()
        {
            // Arrange
            var gameBoard = CreateGameBoard(startingPlayer:PlayerNumber.One);
            
            // Act
            gameBoard.ChangeTurn();

            // Assert
            gameBoard.PlayerTwoStarting.Should().BeFalse();
            gameBoard.GetCurrentPlayer().Should().Be(gameBoard.PlayerTwo);
        }


        [Fact]
        public void NewGame_WhenDefaultSettings_ShouldReturnDefaultArrayOfCells()
        {
            var gameBoard = CreateGameBoard();
            //Arrange
            Cell[] expectedCellArray = new Cell[]
            {
                new Cell("0", 0, false),
                new Cell("1", 1, false),
                new Cell("2", 2, false),
                new Cell("3", 3, false),
                new Cell("4", 4, false),
                new Cell("5", 5, false),
                new Cell("6",6,false),
                new Cell("7",7,false),
                new Cell("8",8,false)
            };
            //Act 
            Cell[] result = gameBoard.NewGame();

            //Assert
            result.Should().BeEquivalentTo(expectedCellArray, options => options.WithStrictOrdering());
        }

        [Fact]
        public void NewGame_ShouldNotReturn_WrongOrderOfCells()
        {
            //Arrange
            var gameBoard = CreateGameBoard();
            Cell[] unExpectedCellArray = new Cell[]
            {
                new Cell("9", 8, false),
                new Cell("8", 7, false),
                new Cell("7", 6, false),
                new Cell("6", 5, false),
                new Cell("5", 4, false),
                new Cell("4", 3, false),
                new Cell("3", 2, false),
                new Cell("2", 1, false),
                new Cell("1", 0, false)
            };

            //Act 
            Cell[] result = gameBoard.NewGame();

            //Assert
            result.Should().NotBeEquivalentTo(unExpectedCellArray, options => options.WithStrictOrdering());
        }

        /// <summary>
        /// Edge case:
        /// When last move out of moves available is made and there is two areas that can be won, return the first win found, should not be draw
        /// </summary>
        [Fact]
        public void MakeMove_WhenLastMoveWinTwoAreas_ShouldBeInWonState()
        {
            //Arrange
            var gameBoard = CreateGameBoard();
            List<int> moveSequence = new() { 1, 4, 2, 5, 3, 7, 6, 8, 0 };
            Cell[] expectedCellArray = new Cell[]
            {
                new Cell("X", 0, true),
                new Cell("X", 1, true),
                new Cell("X", 2, false),
                new Cell("X", 3, false),
                new Cell("O", 4, true),
                new Cell("O", 5, true),
                new Cell("X",6,false),
                new Cell("O",7,false),
                new Cell("O",8,true)
            };

            //Act 
            MakeMovesOnBoard(gameBoard, moveSequence);

            //Assert
            gameBoard.CheckWin().Should().BeTrue();
            gameBoard.Board.Should().BeEquivalentTo(expectedCellArray);
            gameBoard.IsWon.Should().BeTrue();
            gameBoard.IsDraw.Should().BeFalse();
        }

        [Fact]
        public void MakeMove_WhenDiagonalGameWon_ShouldBeInWonState()
        {
            var gameBoard = CreateGameBoard();
            List<int> moveSequence = new() { 0,1,4,5,8};
            //Arrange
            Cell[] expectedCellArray = new Cell[]
            {
                new Cell("X", 0, true),
                new Cell("O", 1, true),
                new Cell("2", 2, false),
                new Cell("3", 3, false),
                new Cell("X", 4, true),
                new Cell("O", 5, true),
                new Cell("6",6,false),
                new Cell("7",7,false),
                new Cell("X",8,true)
            };
            //Act 
            MakeMovesOnBoard(gameBoard, moveSequence);

            //Assert
            gameBoard.CheckWin().Should().BeTrue();
            gameBoard.Board.Should().BeEquivalentTo(expectedCellArray);
            gameBoard.IsWon.Should().BeTrue();
            gameBoard.IsDraw.Should().BeFalse();
        }
        [Fact]
        public void MakeMove_WhenRowGameWon_ShouldBeInWonState()
        {
            //Arrange
            var gameBoard = CreateGameBoard();
            List<int> moveSequence = new() { 3, 0, 4, 1, 5 };

            Cell[] expectedCellArray = new Cell[]
            {
                new Cell("O", 0, true),
                new Cell("O", 1, true),
                new Cell("2", 2, false),
                new Cell("X", 3, true),
                new Cell("X", 4, true),
                new Cell("X", 5, true),
                new Cell("6",6,false),
                new Cell("7",7,false),
                new Cell("8",8,false)
            };

            //Act 
            MakeMovesOnBoard(gameBoard, moveSequence);

            //Assert
            gameBoard.CheckWin().Should().BeTrue();
            gameBoard.Board.Should().BeEquivalentTo(expectedCellArray);
            gameBoard.IsWon.Should().BeTrue();
            gameBoard.IsDraw.Should().BeFalse();
        }

        [Fact]
        public void MakeMove_WhenColumnGameWon_ShouldBeInWonState()
        {
            //Arrange
            var gameBoard = CreateGameBoard();
            List<int> moveSequence = new() { 3, 1, 6, 4, 5, 7 };

            Cell[] expectedCellArray = new Cell[]
            {
                new Cell("0", 0, false),
                new Cell("O", 1, true),
                new Cell("2", 2, false),
                new Cell("X", 3, true),
                new Cell("O", 4, true),
                new Cell("X", 5, true),
                new Cell("X",6,true),
                new Cell("O",7,true),
                new Cell("8",8,false)
            };
            //Act 
            MakeMovesOnBoard(gameBoard, moveSequence);

            //Assert
            gameBoard.CheckWin().Should().BeTrue();
            gameBoard.Board.Should().BeEquivalentTo(expectedCellArray);
            gameBoard.IsWon.Should().BeTrue();
            gameBoard.IsDraw.Should().BeFalse();
        }
    }
}
