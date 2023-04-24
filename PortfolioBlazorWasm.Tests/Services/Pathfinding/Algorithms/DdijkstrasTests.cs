using FluentAssertions;
using Moq;
using PortfolioBlazorWasm.Models.Pathfinding;
using PortfolioBlazorWasm.Models.Pathfinding.Enums;
using PortfolioBlazorWasm.Services.Pathfinding;
using PortfolioBlazorWasm.Services.Pathfinding.Algorithms;
using Xunit;

namespace PortfolioBlazorWasm.Tests.Services.Pathfinding.Algorithms
{
    public class DdijkstrasTests
    {
        private readonly Mock<IPathfindingRunner> _mockPathfindingRunner;

        public DdijkstrasTests()
        {
            _mockPathfindingRunner = new Mock<IPathfindingRunner>();
        }

        [Fact]
        public async Task StartAlgorithm_WithNoWalls_ShouldReturnShortestPath()
        {
            // Arrange
            var grid = GenerateGrid(10, 20);
            Node n1 = new(4, 5, 0, NodeState.Start);
            Node n2 = new(4, 6, 1, NodeState.None);
            Node n3 = new(4, 7, 2, NodeState.None);
            Node n4 = new(4, 8, 3, NodeState.None);
            Node n5 = new(4, 9, 4, NodeState.None);
            Node n6 = new(4, 10, 5, NodeState.Finish);
            Stack<Node> expectedStack = new();
            expectedStack.Push(n6);
            expectedStack.Push(n5);
            expectedStack.Push(n4);
            expectedStack.Push(n3);
            expectedStack.Push(n2);
            expectedStack.Push(n1);
            _mockPathfindingRunner.Setup(m => m.Grid).Returns(grid);
            _mockPathfindingRunner.Object.Grid[4, 5].State = NodeState.Start;
            _mockPathfindingRunner.Object.Grid[4, 10].State = NodeState.Finish;
            bool visitedEventRaised = false;
            bool shortestFoundEventRaised = false;

            _mockPathfindingRunner.Setup(m => m.RaiseVisitedEvent(It.IsAny<object>(), It.IsAny<EventArgs>()))
                                  .Callback(() => visitedEventRaised = true);

            _mockPathfindingRunner.Setup(m => m.RaiseShortestFound(It.IsAny<object>(),
                                                                   It.Is<Stack<Node>>(x => x.SequenceEqual(expectedStack))))
                                  .Callback(() => shortestFoundEventRaised = true);

            var algorithm = new Ddijkstras(_mockPathfindingRunner.Object);

            // Act
            var result = await algorithm.StartAlgorithm(SearchSpeeds.Medium, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            visitedEventRaised.Should().BeTrue();
            shortestFoundEventRaised.Should().BeTrue();
        }
        [Fact]
        public async Task StartAlgorithm_WithNoWalls_ShouldFindShortestPathAndRaiseEvents()
        {
            // Arrange
            var grid = GenerateGrid(10, 20);

            _mockPathfindingRunner.Setup(m => m.Grid).Returns(grid);
            _mockPathfindingRunner.Object.Grid[0, 0].State = NodeState.Start;
            _mockPathfindingRunner.Object.Grid[9, 19].State = NodeState.Finish;
            bool visitedEventRaised = false;
            bool shortestFoundEventRaised = false;
            _mockPathfindingRunner.Setup(m => m.RaiseVisitedEvent(It.IsAny<object>(), It.IsAny<EventArgs>()))
                .Callback(() => visitedEventRaised = true);
            _mockPathfindingRunner.Setup(m => m.RaiseShortestFound(It.IsAny<object>(), It.IsAny<Stack<Node>>()))
                .Callback(() => shortestFoundEventRaised = true);
            var algorithm = new Ddijkstras(_mockPathfindingRunner.Object);

            // Act
            var result = await algorithm.StartAlgorithm(SearchSpeeds.Slow, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            visitedEventRaised.Should().BeTrue();
            shortestFoundEventRaised.Should().BeTrue();
        }
        [Fact]
        public async Task StartAlgorithm_WhenNoPath_ShouldReturnFalse()
        {
            // Arrange
            var grid = GenerateGrid(10, 20);
            _mockPathfindingRunner.Setup(m => m.Grid).Returns(grid);
            _mockPathfindingRunner.Object.Grid[4, 5].State = NodeState.Start;
            _mockPathfindingRunner.Object.Grid[4, 10].State = NodeState.Finish;
            _mockPathfindingRunner.Object.Grid[3, 5].State = NodeState.Wall;
            _mockPathfindingRunner.Object.Grid[4, 4].State = NodeState.Wall;
            _mockPathfindingRunner.Object.Grid[5, 5].State = NodeState.Wall;
            _mockPathfindingRunner.Object.Grid[4, 6].State = NodeState.Wall;

            bool visitedEventRaised = false;
            bool shortestFoundEventRaised = false;
            _mockPathfindingRunner.Setup(m => m.RaiseVisitedEvent(It.IsAny<object>(), It.IsAny<EventArgs>()))
                .Callback(() => visitedEventRaised = true);
            _mockPathfindingRunner.Setup(m => m.RaiseShortestFound(It.IsAny<object>(), It.IsAny<Stack<Node>>()))
                .Callback(() => shortestFoundEventRaised = true);

            var algorithm = new Ddijkstras(_mockPathfindingRunner.Object);

            // Act
            var result = await algorithm.StartAlgorithm(SearchSpeeds.Fast, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            visitedEventRaised.Should().BeFalse();
            shortestFoundEventRaised.Should().BeFalse();
        }
        private Node[,] GenerateGrid(int row, int col)
        {
            Node[,] newGrid = new Node[row, col];
            for (int i = 0; i < newGrid.GetLength(0); i++)
            {
                for (int j = 0; j < newGrid.GetLength(1); j++)
                {
                    newGrid[i, j] = new Node(i, j, int.MaxValue);
                }
            }
            return newGrid;
        }
    }
}
